using DG.Tweening;
using UnityEngine;

namespace Arcube.Animation
{
    public abstract class Animator2D : AnimatorBase
    {
        protected SpriteAnimationClip ActiveClip;
        public override bool Play(ClipInfo clipInfo)
        {
            var clip = clipInfo.clip as SpriteAnimationClip;
            if (!clip) return false;
            ActiveClip = clip;
            Play();
            _tween.onComplete += () =>
            {
                clipInfo.OnComplete?.Invoke();
                CompleteAnimation();
            };
            return true;

        }

        private int _currentIndex; // The starting value of the tween
        private Tween _tween;
        private void Play()
        {
            _lastIndex = -1;
            _currentIndex = 0;
            var target = ActiveClip.sprites.Length - 1;
            _tween = DOTween.To(() => _currentIndex, x => _currentIndex = x, target + ActiveClip.offset, ActiveClip.updateInterval)
             .SetEase(Ease.Linear)
             .OnUpdate(UpdateSprite);
            if (ActiveClip.loopCount != 0)
            {
                _tween.SetLoops(ActiveClip.loopCount, ActiveClip.loopType);
            }
        }

        private int _lastIndex = -1;
        private void UpdateSprite()
        {
            if (_lastIndex == _currentIndex) return;
            if (!ActiveClip) return;

            var sprite = GetClip(_currentIndex);
            if(!sprite)
            {
                Log.AddWarning(()=> $"Sprite not found in {ActiveClip.name}", null, gameObject);
                return;
            }

            SetSprite(sprite);
            _lastIndex = _currentIndex;
        }

        protected virtual Sprite GetClip(int index) => ActiveClip.GetSprite(_currentIndex);

        protected abstract void SetSprite(Sprite sprite);

        private void CompleteAnimation()
        {
            if (_tween?.Loops() != -1) _tween = null;
            if(!ActiveClip) return;

            if (!string.IsNullOrEmpty(ActiveClip.nextClip)) Play(ActiveClip.nextClip);
        }

        public override void Stop() => _tween?.Kill(false);

        protected virtual void Reset() { }

        private void OnDisable() => Stop();
    }
}