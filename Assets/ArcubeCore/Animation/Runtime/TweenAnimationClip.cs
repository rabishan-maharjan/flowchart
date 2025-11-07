using UnityEngine;

namespace Arcube.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimationClip", menuName = "AnimationClip/TweenAnimationClip")]
    public class TweenAnimationClip: AnimationClip
    {
        [TextArea(2, 10)] public string script;
        public PlayMethod playMethod = PlayMethod.All;
    }
}