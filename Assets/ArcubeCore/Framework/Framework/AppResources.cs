using UnityEngine;

namespace Arcube
{
    [CreateAssetMenu(fileName = "AppResources", menuName = "AppData/AppResources", order = 1)]
    public class AppResources : ScriptableObject
    {
        public int appId;
        [Header("App Settings")]
        public bool liveMode = false;
        public int rateReminderCount = 25;
        public string playStoreUrl = "https://play.google.com/store/apps/details?id=com.ArcubeGamesAnimation.Augmee";
        public string appStoreUrl = "https://play.google.com/store/apps/details?id=com.ArcubeGamesAnimation.Augmee";

        [Header("Cache Settings")]
        public int defaultCacheSize = 512;

        [Header("Item Settings")]
        public int[] unlockCost = { 2, 7, 15, 25 };
        public int[] browseCost = { 0, 2, 5, 9 };

        private static AppResources _data;
        public void Init() => _data = this;

        public static string Version => Application.version;
        public static string AppUrl
        {
            get
            {
#if UNITY_ANDROID
                return _data.playStoreUrl;
#elif UNITY_IOS
                return _data.appStoreUrl;
#endif
                return "";
            }
        }
        
        public static int ReminderCount => _data.rateReminderCount;
    }
}