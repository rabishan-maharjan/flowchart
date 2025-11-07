using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private bool isOn;
        [SerializeField] private Graphic on;
        [SerializeField] private Graphic off;

#if UNITY_EDITOR
        private void Reset()
        {
            button = GetComponent<Button>();
            on = transform.GetChild(0).GetComponentInChildren<Graphic>(true);
            off = transform.GetChild(1).GetComponentInChildren<Graphic>(true);
        }
#endif

        public UnityEvent<bool> OnToggle;
        public bool IsOn
        {
            get => isOn;
            set
            {
                SetValue(value);
                OnToggle?.Invoke(isOn);
            }
        }

        public bool Interactable
        {
            get => button.interactable;
            set
            {
                button.interactable = value;
                
                if (on && isOn)
                {
                    on.color = button.interactable ? button.colors.normalColor : button.colors.disabledColor;
                }
                
                if (off && !isOn)
                {
                    off.color = button.interactable ? button.colors.normalColor : button.colors.disabledColor;
                }
            }
        }

        public void SetValue(bool value)
        {
            isOn = value;
            SetUi();
        }

        private void Awake()
        {
            on = on.GetComponentInChildren<Graphic>();
            off = off.GetComponentInChildren<Graphic>();

            button.targetGraphic = isOn ? on : off;

            button.onClick.AddListener(HandleClick);
        }

        private void HandleClick() => IsOn = !IsOn;

        private void SetUi()
        {
            on.gameObject.SetActive(isOn);
            off.gameObject.SetActive(!isOn);

            button.targetGraphic = isOn ? on : off;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetUi();
        }
#endif
    }
}