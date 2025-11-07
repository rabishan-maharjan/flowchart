using DG.Tweening;
using SimpleJSON;
using UnityEngine;

namespace Arcube.Animation
{
    public class DOTweenChildAnimator : AnimatorBase
    {
        [SerializeField] protected Transform[] targets;
        [SerializeField] protected float delay = 0;
        [SerializeField] private bool randomize = true;
        protected void Reset()
        {
            targets = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
            {
                targets[i] = transform.GetChild(i);
            }
        }

        public override bool Play(ClipInfo clipInfo)
        {
            var clip = clipInfo.clip as TweenAnimationClip;
            var anim = JSONNode.Parse(clip.script);
            var i = 0;
            if(randomize) targets = Utils.RandomizeArray(targets);
            foreach (Transform t in targets)
            {
                if (clip.playMethod == PlayMethod.Sequential)
                {
                    DOTweenWrapper.PlaySequence(anim, transform).SetDelay(delay * i++);
                }
                else
                {
                    DOTweenWrapper.PlayAll(anim, t).SetDelay(delay * i++);
                }
            }

            return true;
        }

        private void OnDisable() => Stop();

        public override void Stop()
        {
        }
    }
}