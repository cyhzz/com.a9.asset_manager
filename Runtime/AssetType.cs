using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.A9.AssetManager
{
    public static class AssetType
    {
        public static Type GetAssetType(string assetName)
        {
            if (AssetType.IsPrefabFile(assetName))
            {
                return typeof(GameObject);
            }
            else if (AssetType.IsSpriteSheetFile(assetName))
            {
                return typeof(IList<Sprite>);
            }
            else if (AssetType.IsTextureFile(assetName))
            {
                return typeof(Sprite);
            }
            else if (AssetType.IsAudioFile(assetName))
            {
                return typeof(AudioClip);
            }
            else if (AssetType.IsTextFile(assetName))
            {
                return typeof(TextAsset);
            }
            else if (AssetType.IsMaterialFile(assetName))
            {
                return typeof(Material);
            }

            return typeof(UnityEngine.Object);
        }

        public static bool IsPrefabFile(string path)
        {
            return path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsTextFile(string path)
        {
            return path.EndsWith(".bytes", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAudioFile(string path)
        {
            return path.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsMaterialFile(string path)
        {
            return path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsTextureFile(string path)
        {
            return path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".tga", StringComparison.OrdinalIgnoreCase)
                   || path.EndsWith(".psd", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSpriteSheetFile(string path)
        {
            return IsTextureFile(path) && path.Contains("sheet");
        }

        public static bool IsShaderFile(string path)
        {
            return path.EndsWith(".shader", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAnimFile(string path)
        {
            return path.EndsWith(".anim", StringComparison.OrdinalIgnoreCase);
        }
    }
}
