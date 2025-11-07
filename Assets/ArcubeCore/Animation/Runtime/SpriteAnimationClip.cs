using UnityEngine;
using UnityEngine.Serialization;

namespace Arcube.Animation
{
    [CreateAssetMenu(fileName = "SpriteAnimationClip", menuName = "AnimationClip/SpriteAnimationClip")]
    public class SpriteAnimationClip: AnimationClip
    {
        public Sprite[] sprites;
        public int offset = 0;

        [FormerlySerializedAs("speed")] public float updateInterval = 0.1f;
        public string nextClip;

        private int index = -1;
        private int count = 0;

        private int loopCounter;
        private bool loop;
        public void Init()
        {
            loopCounter = loopCount;
            index = offset;
            loop = loopCount != 0;
        }
        public Sprite GetSprite()
        {
            loop = loopCounter != 0;
            if (!loop)
            {
                if (++count == sprites.Length)
                {
                    index = -1;
                    return null;
                }
            }

            index++;
            if(index == offset && loopCounter > 0)
            {
                loopCounter--;
            }
            if (index >= sprites.Length) index = 0;

            return sprites[index];
        }

        public Sprite GetSprite(int value)
        {
            var index = (value + offset) % sprites.Length;
            return sprites[index];
        }
    }
}