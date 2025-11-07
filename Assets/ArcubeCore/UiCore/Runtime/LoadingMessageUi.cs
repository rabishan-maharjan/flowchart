using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Arcube.UiManagement
{
    public class LoadingMessageUi : Ui
    {
        [SerializeField] private TMP_Text t_message;
        protected override void Reset()
        {
            base.Reset();
            t_message = GetComponentInChildren<TMP_Text>();
            deactivateOnClose = true;
        }

        private Transform _initParent;
        public static LoadingMessageUi Instance { get; private set; }
        public override Task Register()
        {
            _initParent = transform.parent;
            Instance = this;
            return base.Register();
        }

        public static void Show(string message, Transform parent = null)
        {
            if (!Instance) return;

            Instance.Set(message, parent);
        }

        private void Set(string message, Transform parent = null)
        {
            if(!parent) parent = _initParent;

            transform.SetParent(parent);
            transform.SetAsLastSibling();
            var rt = transform as RectTransform;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
            transform.localScale = Vector3.one;
            t_message.text = message;
            Open();
        }

        public static void Hide()
        {
            if(!Instance) return; 
            Instance.Close();
        }
    }
}