using UnityEngine;
using UnityEngine.UI;

namespace Arcube.Animation
{
    public class ImageAnimator : Animator2D
    {
        [SerializeField] private Image image;

        protected override void SetSprite(Sprite sprite) => image.sprite = sprite;

        protected override void Reset() => image = GetComponentInChildren<Image>();
    }
}