using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Arcube
{
    public enum AppState
    {
        Setup,
        Initialized,
        Ended
    }

    public abstract class AppManager : ManagerBase
    {
        private static AppManager _instance;

        public static AppManager Instance
        {
            get
            {
                if (!_instance) _instance = FindAnyObjectByType<AppManager>();
                return _instance;
            }
            private set => _instance = value;
        }

        public DateTime LastPlayedDate { get; private set; }
        protected bool LoadOnlineServices = true;
        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        [FormerlySerializedAs("_managers")] [SerializeField] protected List<ManagerBase> managers;
        public List<ManagerBase> Managers { get => managers; set => managers = value; }
        public static T GetManager<T>() where T : ManagerBase
        {
            if (!Instance) return null;

            var manager = Instance.Managers.FirstOrDefault(m => m is T);
            if (manager) return manager as T;
            return null;
        }
        
        public static bool TryGetManager<T>(out T manager) where T : ManagerBase
        {
            manager = null;
            if(!Instance)  return false;
            manager = (T)Instance.Managers.FirstOrDefault(m => m is T);
            return manager;
        }

        public static event Action<AppState> OnStateChanged;
        private AppState _state;
        public AppState State
        {
            get => _state;
            set
            {
                if (_state == value) return;

                OnStateChanged?.Invoke(value);
                _state = value;
            }
        }

        public bool IsPremium { get; set; }
        public int PlayCount { get; private set; }
        public override async Task Register()
        {
            try
            {
                PlayCount = PlayerPrefs.GetInt("play_count", 0);
                PlayerPrefs.SetInt("play_count", PlayCount + 1);
                var lastPlayedDate = PlayerPrefs.GetString("last_played_date");
                LastPlayedDate = string.IsNullOrEmpty(lastPlayedDate) ? DateTime.Now : JsonConvert.DeserializeObject<DateTime>(lastPlayedDate);
                
                PlayerPrefs.SetString("last_played_date", JsonConvert.SerializeObject(DateTime.Now));
                
                foreach (var manager in managers)
                {
                    await manager.Register();
                    Log.Add(()=> "Registered manager: " + manager.GetType().Name);
                };
            }
            catch(Exception e)
            {
                Log.AddException(e);
            }
        }

        public override async Task<bool> Initialize()
        {
            try
            {
                foreach (var manager in managers)
                {
                    if (manager.Initialized)
                    {
                        //Log.Add(() => "Manager already initialized: " + manager.GetType().Name);
                        continue;
                    }
                    
                    if (!LoadOnlineServices && manager.IsOnlineService) continue;
                    await manager.Initialize();
                    Log.Add(() => "Manager initialized: " + manager.GetType().Name);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.AddException(e);
                return false;
            }
        }
    }
}