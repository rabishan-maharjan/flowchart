using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Arcube.Api
{
    public enum UrlKey
    {
        MaintenanceMode,
        Time,
        Offers,
        Songs,
        ReadNotes,
        UpdatePlayCount,
        IncrementLikes,
        DecrementLikes,
    }

    [CreateAssetMenu(menuName = "AppData/Urls", fileName = "Urls")]
    public class Urls : ScriptableObject
    {
        public string domain = "https://ludo.arcube.com.np";
        public string version = "v1";
        [SerializedDictionary("key", "address")]
        public SerializedDictionary<UrlKey, string> urls;
        public string GetDataUrl(string key, string parameter = "") => $"{domain}/storage/{key}{parameter}";
        public string GetApiURL(UrlKey key, string parameter = "") => $"{domain}/api{version}/{urls[key]}{parameter}";
    }
}