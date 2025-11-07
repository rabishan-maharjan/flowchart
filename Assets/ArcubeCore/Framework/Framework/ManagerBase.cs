using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Arcube
{
    public class ManagerBase : MonoBehaviour, IManager
    {
        protected bool IsInitializing { get; set; }
        public event Action OnInitializationSuccess;
        [field: SerializeField] public bool IsOnlineService { get; private set; }
        public bool Initialized { get; protected set; }

        public virtual async Task Register()
        {
            await Task.Run(() => { });
        }

        public virtual async Task<bool> Initialize()
        {
            Initialized = true;
            OnInitializationSuccess?.Invoke();
            return true;
        }
    }
}