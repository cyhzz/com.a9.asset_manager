using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.A9.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Com.A9.AssetManager
{
    public class ReadRequest
    {
        public string path;
        public string name;
        public AsyncOperationHandle handle;

        public T GetResult<T>()
        {
            return (T)handle.Result;
        }

        public bool IsDone()
        {
            return handle.IsValid() && handle.IsDone;
        }
    }

    public class AssetManager : Singleton<AssetManager>
    {
        public Dictionary<string, ReadRequest> dic = new Dictionary<string, ReadRequest>();

        public string GetName(string path)
        {
            string[] parse = path.Split('/');
            string[] parse_2 = parse[parse.Length - 1].Split('.');
            return parse_2[0];
        }

        public ReadRequest Load<T>(string path, Action OnAllComplete = null)
        {
            if (dic.ContainsKey(path))
            {
                var req = dic[path];
                if (req.handle.IsValid() && req.handle.Result != null)
                {
                    OnAllComplete?.Invoke();
                    return req;
                }
                else
                {
                    StartCoroutine(Load_(new List<string>() { path }));
                    return req;
                }
            }
            var re = new ReadRequest() { path = path, name = GetName(path) };
            dic.Add(path, re);
            StartCoroutine(Load_(new List<string>() { path }));
            return re;
        }

        public void Load(List<string> paths, Action OnAllComplete = null)
        {
            var path2 = new List<string>();
            for (int i = 0; i < paths.Count; i++)
            {
                if (dic.ContainsKey(paths[i]))
                {
                    var req = dic[paths[i]];
                    if (req.handle.IsValid() && req.handle.Result != null)
                    {
                        continue;
                    }
                    else
                    {
                        path2.Add(paths[i]);
                    }
                }
                else
                {
                    path2.Add(paths[i]);
                }
            }
            StartCoroutine(Load_(path2, OnAllComplete));
        }

        IEnumerator Load_(List<string> paths, Action OnAllComplete = null)
        {
            AsyncOperationHandle<IList<IResourceLocation>> locations =
                   Addressables.LoadResourceLocationsAsync(paths,
                   Addressables.MergeMode.Union);
            yield return locations;

            var loads = new List<AsyncOperationHandle>(locations.Result.Count);
            foreach (var location in locations.Result)
            {
                Debug.Log(location.PrimaryKey);
                Debug.Log(AssetType.GetAssetType(location.PrimaryKey));

                AsyncOperationHandle handle = new AsyncOperationHandle();
                if (AssetType.GetAssetType(location.PrimaryKey) == typeof(GameObject))
                    handle = Addressables.LoadAssetAsync<GameObject>(location);
                else if (AssetType.GetAssetType(location.PrimaryKey) == typeof(IList<Sprite>))
                    handle = Addressables.LoadAssetAsync<IList<Sprite>>(location.PrimaryKey);
                else if (AssetType.GetAssetType(location.PrimaryKey) == typeof(Sprite))
                    handle = Addressables.LoadAssetAsync<Sprite>(location.PrimaryKey);
                else if (AssetType.GetAssetType(location.PrimaryKey) == typeof(AudioClip))
                    handle = Addressables.LoadAssetAsync<AudioClip>(location.PrimaryKey);
                else if (AssetType.GetAssetType(location.PrimaryKey) == typeof(Material))
                    handle = Addressables.LoadAssetAsync<Material>(location.PrimaryKey);

                // handle.Completed+=obj=>
                handle.Completed += obj =>
                {
                    if (dic.ContainsKey(location.PrimaryKey))
                    {
                        dic[location.PrimaryKey].handle = handle;
                    }
                    else
                    {
                        var nw_req = new ReadRequest()
                        {
                            path = location.PrimaryKey,
                            handle = handle,
                            name = GetName(location.PrimaryKey)
                        };
                        dic.Add(location.PrimaryKey, nw_req);
                        if (AssetType.GetAssetType(location.PrimaryKey) == typeof(AudioClip))
                        {
                            Debug.Log($"Load succ {location.PrimaryKey} {nw_req.GetResult<AudioClip>().name}");
                        }
                    }
                };
                loads.Add(handle);
            }
            yield return Addressables.ResourceManager.CreateGenericGroupOperation(loads, true);
            OnAllComplete?.Invoke();
        }

        public T Get<T>(string str) where T : class
        {
            foreach (var item in dic)
            {
                var req = item.Value;
                if (req.handle.Result != null)
                {
                    if (item.Value.name == str)
                        return (T)req.handle.Result;
                }
            }
            return null;
        }

        public void Unload(string path)
        {
            if (dic.ContainsKey(path) == false)
            {
                return;
            }
            Addressables.Release(dic[path].handle);
        }
    }

}

