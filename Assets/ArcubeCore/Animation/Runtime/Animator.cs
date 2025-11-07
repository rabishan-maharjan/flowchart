using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Arcube.Animation
{
    public abstract class AnimationClip : ScriptableObject
    {
        public int loopCount = 0;
        [FormerlySerializedAs("looptype")] public LoopType loopType = LoopType.Yoyo;
        public Ease easeType = Ease.Linear;
    }

    public abstract class AnimatorBase: MonoBehaviour
    {
        [Serializable]
        public class ClipInfo
        {
            public string name;
            [Expandable] public AnimationClip clip;
            public UnityEvent OnComplete;
        }

        private void OnValidate()
        {
            if (clipInfos == null) return;

            foreach (var clipInfo in clipInfos)
            {
                if (clipInfo.clip != null && string.IsNullOrEmpty(clipInfo.name)) clipInfo.name = clipInfo.clip.name;
            }
        }

        [SerializeField] protected ClipInfo[] clipInfos;        
        [field: SerializeField] protected string[] defaultAnimations { get; set; }

        public ClipInfo GetClip(string key)
        {
            return clipInfos.FirstOrDefault(c => c.name == key);
        }

        public ClipInfo[] GetClips(string key)
        {
            return clipInfos.Where(c => c.name == key).ToArray();
        }

        protected virtual void OnEnable() => PlayDefault();

        public void PlaySimple(string clipName) => Play(clipName);

        public virtual AnimatorBase Play(string clipName)
        {
            foreach (var clip in GetClips(clipName))
            {
                Play(clip);
            }

            return this;
        }

        public abstract bool Play(ClipInfo clipInfo);

        private void PlayDefault()
        {
            foreach(var clip in defaultAnimations)
            {
                Play(clip);
            }
        }

        public abstract void Stop();
    }
}