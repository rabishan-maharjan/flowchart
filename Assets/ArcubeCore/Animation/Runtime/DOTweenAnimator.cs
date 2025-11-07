using SimpleJSON;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

namespace Arcube.Animation
{
    public enum PlayMethod
    {
        Sequential,
        All,
    }

    public class DOTweenAnimator : AnimatorBase
    {
        private readonly Dictionary<ClipInfo, Sequence> _activeClips = new();        
        public override bool Play(ClipInfo clipInfo)
        {
            var clip = clipInfo.clip as TweenAnimationClip;
            Sequence sequence;
            if (_activeClips.TryGetValue(clipInfo, out var activeClip))
            {
                sequence = activeClip;
                if (sequence.IsPlaying()) return false;
            }

            var anim = JSONNode.Parse(clip.script);
            sequence = (clip.playMethod == PlayMethod.Sequential) ? DOTweenWrapper.PlaySequence(anim, transform) : DOTweenWrapper.PlayAll(anim, transform);
            _activeClips[clipInfo] = sequence;

            if (clip.loopCount != 0)
            {
                sequence.SetLoops(clip.loopCount, clip.loopType);
            }

            sequence.SetEase(clip.easeType);

            sequence.onComplete += () =>
            {
                clipInfo.OnComplete?.Invoke();
            };

            sequence.onKill += () =>
            {
                _activeClips.Remove(clipInfo);
            };

            return true;
        }

        public override void Stop()
        {
            var sequences = _activeClips.Values.ToArray();
            for (var i = sequences.Length - 1; i >= 0; i--)
            {
                Stop(sequences[i]);
            }

            _activeClips.Clear();
        }

        private void Stop(Sequence sequence)
        {
            sequence?.Kill();
        }

        private void OnDisable() => Stop();

        private void OnDestroy()
        {
            foreach (var clip in _activeClips)
            {
                clip.Value?.Kill();
            }
        }
    }
}