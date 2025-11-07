using System;
using System.Linq;
using UnityEngine;

namespace Common.Ui.Animation
{
    [Serializable]
    public class Clip
    {
        public string name;
        public Sprite[] sprites;
        public float speed = 0.1f;
        public bool loop = false;
        public string nextClip;

        public float Duration => speed * sprites.Length;
    }

    public class Animator2D : MonoBehaviour
    {
        public Clip[] clips;

        [SerializeField] private bool playOnAwake = false;
        public virtual void Start()
        {
            if (!playOnAwake) return;
            if (clips == null || clips.Length <= 0) return;
            PlayDefault();
        }

        protected Clip activeClip;
        public virtual void SetDefault() => activeClip = clips[0];

        public Animator2D Play(string name)
        {
            var result = clips.FirstOrDefault(c => c.name == name);
            activeClip = result;
            Play();
            return this;
        }

        private bool Playing { get; set; } = false;
        public void Play(Clip clip)
        {
            activeClip = clip;
            Play();
        }

        public virtual void PlayDefault() => Play(clips[0]);

        private float timer = 0;
        private void Play()
        {
            Playing = true;
            OnComplete = null;

            if (activeClip == null) return;

            timer = activeClip.Duration;
        }

        public void Pause()
        {
            Playing = false;
        }

        public void Resume()
        {
            Playing = true;
        }

        private void Update()
        {
            if (!Playing || activeClip == null) return;
            if (timer <= 0)
            {
                if (!activeClip.loop)
                {
                    CompleteAnimation();
                    return;
                }
                else
                {
                    timer = activeClip.Duration;
                }
            }

            timer -= Time.deltaTime;
            if (timer < 0) timer = 0;
            UpdateAnimation((activeClip.Duration - timer) / activeClip.Duration);
        }

        protected virtual void UpdateAnimation(float time) { }

        public Action OnComplete;
        private void CompleteAnimation()
        {
            Playing = false;
            OnComplete?.Invoke();

            if (string.IsNullOrEmpty(activeClip.nextClip)) return;

            Play(activeClip.nextClip);
        }

        public virtual void Stop() => Playing = false;

        protected virtual void Reset() { }
    }
}