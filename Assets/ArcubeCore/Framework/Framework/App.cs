using System;
using NaughtyAttributes;
using UnityEngine;

namespace Arcube
{
    public class App : MonoBehaviour
    {
        public static App Instance { get; private set; }
        [field: SerializeField] public AppResources AppResources { get; private set; }
        [field: SerializeField] private ManagerBase UIManager { get; set; }
        [field: SerializeField] public ManagerBase AppManager { get; private set; }

        private void Reset()
        {
            UIManager = gameObject.FindObject<ManagerBase>("UiManager");
            AppManager = gameObject.FindObject<ManagerBase>("AppManager");
        }
#if UNITY_EDITOR
        public string pass;
        
        [Button]
        public void ClearPlayerPrefs() => PlayerPrefs.DeleteAll();

        [Button]
        public void ShowSaveFolder() => Application.OpenURL(Application.persistentDataPath);
#endif
        private void Awake()
        {
            Instance = this;
        
            Application.runInBackground = false;
            Application.targetFrameRate = 60;
        }

        public event Action OnAppStarted;
        private async void Start()
        {
            try
            {
                Log.Add(() => "Registering");                
                await AppManager.Register();
                await UIManager.Register();

                Log.Add(() => "Initializing");
                await AppManager.Initialize();
                await UIManager.Initialize();
                
                OnAppStarted?.Invoke();
            }
            catch (Exception e)
            {
                Log.AddException(e);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //exit full screen mode
                Screen.fullScreen = false;
            }
        }
    }
}