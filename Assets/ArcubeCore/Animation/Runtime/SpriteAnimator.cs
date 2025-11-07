using UnityEngine;

namespace Arcube.Animation
{
    public class SpriteAnimator : Animator2D
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        protected override void Reset() => spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        protected override void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;
    }
}