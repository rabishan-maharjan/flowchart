using Arcube.AssetManagement;
using Arcube.UiManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Arcube
{
    public abstract class UiManager : ManagerBase
    {
        [FormerlySerializedAs("_uis")] [SerializeField] protected List<Ui> uis;
        public List<Ui> Uis
        {
            get => uis;
            private set => uis = value;
        }

        protected virtual void Reset() => uis = new List<Ui>(GetComponentsInChildren<Ui>(true));

        public static T GetUi<T>() where T : Ui
        {
            var ui = Instance.Uis.FirstOrDefault(m => m is T);
            if (!ui)
            {
                Log.AddError(()=> $"Ui missing {typeof(T)}");
            }

            return ui as T;
        }

        public static bool TryGetUi<T>(out T result) where T : Ui
        {
            result = Instance.Uis.FirstOrDefault(m => m is T) as T;
            if (result) return true;
            Log.AddWarning(()=> "Ui missing");
            return false;
        }

        public static UiManager Instance { get; private set; }
        protected virtual void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public override async Task Register()
        {
            foreach (var ui in uis)
            {
                await ui.Register();
            }
        }

        public override async Task<bool> Initialize()
        {
            foreach (var ui in uis)
            {
                Log.AddPriority(() => $"Initializing {ui.name}");
                await ui.Initialize(null);
            }

            return true;
        }

        public static async Task CreateAndOpen<T>() where T : Ui
        {
            if (Instance.Uis.FirstOrDefault(m => m is T)) return;

            var ui = await AssetManager.Instantiate<T>($"{typeof(T).Name}", Instance.transform) as Ui;
            if (!ui)
            {
                Log.AddError(()=> $"Asset not found Ui/{typeof(T).Name}");
                return;
            }

            ui.name = typeof(T).Name;

            await ui.OpenAsync();

            AssetManager.ReleaseInstance(ui.gameObject);
        }
        public static async Task<T> Create<T>() where T : Ui
        {
            var result = Instance.Uis.FirstOrDefault(m => m is T);
            if (result) return result as T;

            var ui = await AssetManager.Instantiate<T>($"{typeof(T).Name}", Instance.transform) as Ui;
            if (!ui)
            {
                Log.AddError(()=> $"Asset not found Ui/{typeof(T).Name}");
                return null;
            }

            ui.name = typeof(T).Name;
            return ui.GetComponent<T>();
        }
        
        public static void AddUi(Ui ui)
        {
            if (!Instance) return;

            if (!Instance.Uis.Contains(ui))
            {
                Instance.Uis.Add(ui);
            }
        }

        public static void RemoveUi(Ui ui)
        {
            if (!Instance) return;

            Debug.Log("Removing " + ui.name);
            if (Instance.Uis.Contains(ui))
            {
                Instance.Uis.Remove(ui);
                Debug.Log("Removed " + ui.name);
            }
        }

        private List<Ui> ActiveUis { get; set; } = new List<Ui>();
        private Ui ActiveUi { get; set; }

        public void AddActiveUi(Ui ui)
        {
            ActiveUis.Add(ui);
            SetActiveUi();
        }

        internal void RemoveActiveUi(Ui ui)
        {
            ActiveUis.Remove(ui);
            SetActiveUi();
        }

        private void SetActiveUi()
        {
            var index = ActiveUis.Count - 1;
            if (index < 0) return;

            ActiveUi = ActiveUis[index];
        }

        private void Update()
        {
            if (!ActiveUi || !ActiveUi.Closable) return;

            if (Input.GetKeyDown(KeyCode.Escape)) ActiveUi.Close();
        }
    }
}