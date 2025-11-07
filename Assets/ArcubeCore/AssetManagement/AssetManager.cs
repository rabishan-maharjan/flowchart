using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Arcube.AssetManagement
{
    public static class AssetManager
    {
        public static async Task Init()
        {
            ResourceManager.ExceptionHandler = CustomExceptionHandler;
            await Addressables.InitializeAsync().Task;
        }

        public static async Task DownloadByLabel(string label, Action<float> progressCallback)
        {
            // Check total size first
            var sizeHandle = Addressables.GetDownloadSizeAsync(label);
            await sizeHandle.Task;

            if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var totalSize = sizeHandle.Result;
                if (totalSize > 0)
                {
                    var downloadHandle = Addressables.DownloadDependenciesAsync(label);
                    while (!downloadHandle.IsDone)
                    {
                        progressCallback?.Invoke(downloadHandle.PercentComplete);
                        await Task.Yield();
                    }

                    if (downloadHandle.Status == AsyncOperationStatus.Failed)
                    {
                        progressCallback?.Invoke(-1);
                    }

                    Addressables.Release(downloadHandle);
                }
            }
            else
            {
                progressCallback?.Invoke(-1);
            }

            Addressables.Release(sizeHandle);
        }

        private static void CustomExceptionHandler(AsyncOperationHandle handle, Exception exception) =>  Log.Add(()=> exception.Message);

        private static async Task<IList<IResourceLocation>> LoadLocationsAsync(string key)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key);
            var location = await handle.Task;

            if (location.Count <= 0) return null;

            Addressables.Release(handle);
            return location;
        }
        public static async Task<float> CheckDownloadSizeAsync(string key)
        {
            var location = await LoadLocationsAsync(key);
            if (location == null) return -1;

            var handle = Addressables.GetDownloadSizeAsync(location);
            var size = await handle.Task;
            Addressables.Release(handle);
            return size / 1024.0f / 1024;
        }

        public static async Task<T> LoadAsset<T>(string v) => await Addressables.LoadAssetAsync<T>(v).Task;
        public static async Task<T> LoadAsset<T>(AssetReference reference) => await reference.LoadAssetAsync<T>().Task;

        public static async Task<T> Instantiate<T>(string key, Transform parent) where T : Component
        {
            var result = await Addressables.InstantiateAsync(key, parent).Task;
            return result.GetComponent<T>();
        }

        public static async Task<T> InstantiateAsset<T>(AssetReference asset, Transform parent) where T : Component
        {
            var result = await Addressables.InstantiateAsync(asset, parent).Task;
            return result.GetComponent<T>();
        }

        public static void Release(object asset) => Addressables.Release(asset);
        public static void ReleaseInstance(GameObject asset)
        {
            Addressables.ReleaseInstance(asset);

            Resources.UnloadUnusedAssets();
        }
    }
}