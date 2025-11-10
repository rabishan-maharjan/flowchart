using Arcube.Animation;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Arcube.UiManagement
{
    public enum UiState
    {
        Opening,
        Opened,
        Closing,
        Closed
    }

    public abstract class Ui : MonoBehaviour
    {
        [SerializeField] protected Canvas canvas;
        [SerializeField] private DOTweenAnimator animator;
        protected virtual void Reset()
        {
            canvas = GetComponentInParent<Canvas>();
            animator = GetComponentInChildren<DOTweenAnimator>();
        }

        protected virtual void Awake() => UiManager.AddUi(this);

        public virtual async Task Register()
        {
            if (deactivateOnClose && State == UiState.Closed) gameObject.SetActive(false);
            await Task.Run(() => { });
        }

        /// <summary>
        /// send in null if manager class not required
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public virtual async Task Initialize(ManagerBase manager) => await Task.Run(() => { });

        public UiState State { get; protected set; } = UiState.Closed;

        private static List<Ui> ActiveUis { get; set; } = new();

        /// <summary>
        /// open the ui
        ///  pass parameter if you want to be able to return back to and auto close current ui
        /// </summary>
        public virtual void Open()
        {
            if (State != UiState.Closed) return;

            State = UiState.Opening;

            gameObject.SetActive(true);

            SetUi();

            if (animator)
            {
                if (!animator.Play("Open"))
                {
                    OnOpen();
                }
            }
            else 
            {
                OnOpen();
            }
        }
        
        public virtual async Task OpenAsync()
        {
            Open();

            while(State != UiState.Closed)
            {
                await Task.Yield();
                destroyCancellationToken.ThrowIfCancellationRequested();
            }
        }

        [SerializeField] protected bool closable = false;
        public bool Closable => closable;
        public virtual void OnOpen()
        {
            State = UiState.Opened;

            if (TryGetComponent(out CanvasGroup c))
            {
                c.blocksRaycasts = true;
                c.alpha = 1;
            }

            ActiveUis.Add(this);
        }

        protected virtual void SetUi() { }

        public virtual void Close()
        {            
            if (!gameObject.activeSelf) return;
            State = UiState.Closing;
         
            if (animator)
            {
                if (!animator.Play("Close"))
                {
                    OnClose();
                }
            }
            else
            {
                OnClose();
            }
        }

        [SerializeField] protected bool deactivateOnClose = false;
        public virtual void OnClose()
        {
            if (TryGetComponent(out CanvasGroup c))
            {
                c.blocksRaycasts = false;
                c.alpha = 0;
            }

            if (deactivateOnClose) gameObject.SetActive(false);
            State = UiState.Closed;

            ActiveUis.Remove(this);
        }

        public virtual void Toggle()
        {
            if (State == UiState.Opened)
            {
                Close();
            }
            else if (State == UiState.Closed)
            {
                Open();
            }
        }

        public void Back() => Close();

        protected virtual void OnDestroy() => UiManager.RemoveUi(this);

        private void Update()
        {
            if (!Closable || State != UiState.Opened) return;
            if (Input.GetKeyDown(KeyCode.Escape)) Back();
        }
    }
}