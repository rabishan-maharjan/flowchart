using UnityEngine;

namespace Common.Ui.Animation
{
    public class SpriteAnimator : Animator2D
    {
        [SerializeField] private SpriteRenderer _renderer;
        public override void SetDefault()
        {
            base.SetDefault();
            _renderer.sprite = activeClip.sprites[0];
        }

        protected override void UpdateAnimation(float time)
        {
            if (_renderer == null) return;
            var i = Mathf.CeilToInt(activeClip.sprites.Length * time) - 1;
            _renderer.sprite = activeClip.sprites[i];
        }

        protected override void Reset()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>(true);
            _renderer.gameObject.SetActive(true);
        }

        public override void PlayDefault()
        {
            Reset();
            base.PlayDefault();
        }
        public override void Stop()
        {
            base.Stop();
            if (_renderer == null) return;

            _renderer.gameObject.SetActive(false);
            _renderer = null;            
        }
    }
}