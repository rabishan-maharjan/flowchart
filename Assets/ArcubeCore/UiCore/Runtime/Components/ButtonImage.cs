using Arcube.Animation;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("ArcubeUI/ButtonImage")]
    public class ButtonImage : MonoBehaviour
    {
        [FormerlySerializedAs("_button")] [SerializeField] private Button button; 
        //[SerializeField] private bool playSound = true;
        [field: SerializeField] public Image Image { get; private set; }
        
        public bool Interactable
        {
            get => button.interactable;
            set => button.interactable = value;
        }
        
        protected virtual void Reset()
        {
            button = GetComponent<Button>();
            Image = GetComponent<Image>();
        }
        
        protected virtual void Awake()
        {
            if (TryGetComponent<DOTweenAnimator>(out var animator))
            {
                var clipInfo = animator.GetClip("Click");
                clipInfo.OnComplete.AddListener(() =>
                {
                    OnClick?.Invoke();
                });

                button.onClick.AddListener(() =>
                {
                    animator.Play("Click");
                    //if (playSound) SoundManager.PlaySfx("Click");
                });
            }
            else
            {
                if (button.onClick == null) return;

                button.onClick = OnClick;
                button.onClick.AddListener(() =>
                {
                    //if (playSound) SoundManager.PlaySfx("Click");
                });
            }
        }

        public Button.ButtonClickedEvent OnClick => button.onClick;
    }
}