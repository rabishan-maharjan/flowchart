using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Arcube.UiManagement
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldSimple : MonoBehaviour
    {
        [SerializeField] private ButtonText b_clear;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text placeHolder;
        public TMP_InputField.OnChangeEvent onValueChanged => inputField.onValueChanged;
        public string PlaceHolder
        {
            set => placeHolder.text = value;
        }
        
        public bool Interactable
        {
            set => inputField.interactable = value;
        }
        
        public TMP_InputField.ContentType ContentType
        {
            set => inputField.contentType = value;
        }

        public string Text
        {
            get => inputField.text;
            set => inputField.text = value;
        }
        
        protected void Reset()
        {
            inputField = GetComponent<TMP_InputField>();
            placeHolder = gameObject.FindObject<TMP_Text>("Placeholder");
            b_clear = gameObject.FindObject<ButtonText>("b_clear");
        }
        
        protected void Start()
        {
            b_clear.OnClick.AddListener(Clear);
            inputField.placeholder.gameObject.SetActive(string.IsNullOrEmpty(Text));
        }

        private void Clear() => Text = "";
    }
}