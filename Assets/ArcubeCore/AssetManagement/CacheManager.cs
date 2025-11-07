using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Arcube.AssetManagement
{
    public static class CacheManager
    {
        //public static float MaxSize = 1024;
        //public static async Task Init(IList<IResourceLocation> models)
        //{
        //    Addressables.CleanBundleCache();
            
        //    var size = GetCacheSize();
        //    if (size <= MaxSize) return;

        //    var i = 0;
        //    do
        //    {
        //        var key = models[i].PrimaryKey;
        //        var modelSize = await AssetManager.CheckDownloadSizeAsync(key);
        //        if(modelSize > 0) await Delete(key);
        //        size = GetCacheSize();
        //    } while (size > MaxSize && i < models.Count);
        //}

        //public static void Load()
        //{
        //    var metas = new List<Meta>();
        //    foreach (var filePath in Directory.GetFiles(Application.persistentDataPath))
        //    {
        //        var key = Path.GetFileNameWithoutExtension(filePath);
        //        var meta = Meta.LoadOrCreate(key);
        //        metas.Add(meta);
        //    }
        //}

        //public static void SetCacheSize(int index)
        //{
        //    MaxSize = index * AppResources.DefaultCacheSize;
        //    Save(index);
        //}

        //public static void Save(int cacheIndex) => PlayerPrefs.SetInt("CacheIndex", cacheIndex);

        //public static int GetCacheIndex() => PlayerPrefs.GetInt("CacheIndex", 0);

        //public static float GetCacheSize()
        //{
        //    var path = Utils.AddressablePath();
        //    return Utils.GetDirectorySize(path) / 1024 / 1024.0f;
        //}

        //public static async Task<bool> Delete(string key) => await Addressables.ClearDependencyCacheAsync(key, true).Task;
    }
}