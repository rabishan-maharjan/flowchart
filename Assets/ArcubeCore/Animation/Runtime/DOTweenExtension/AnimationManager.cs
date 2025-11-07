using DG.Tweening;
using SimpleJSON;
using UnityEngine;

namespace Arcube.Animation
{
    public static class AnimationManager
    {
        public static Sequence Animate(string animation, Transform obj)
        {
            if (string.IsNullOrEmpty(animation)) return null;
            JSONNode node = JSON.Parse(animation);
            return DOTweenWrapper.PlaySequence(node, obj).Play();
        }

        public static Sequence Animate(JSONNode node, Transform obj)
        {
            return DOTweenWrapper.PlaySequence(node, obj.transform).Play();
        }

        public static Sequence Animate(TweenBuilder.Animation animation, Transform obj)
        {
            return DOTweenWrapper.PlaySequence(animation, obj.transform).Play();
        }
    }
}