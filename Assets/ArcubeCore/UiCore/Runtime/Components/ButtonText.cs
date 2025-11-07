using TMPro;
using UnityEngine;

namespace Arcube.UiManagement
{
    [AddComponentMenu("ArcubeUI/ButtonText")]
    public class ButtonText : ButtonImage
    {
        [SerializeField] private TMP_Text _text;
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            _text = GetComponentInChildren<TMP_Text>();
        }
#endif
        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }
    }
}