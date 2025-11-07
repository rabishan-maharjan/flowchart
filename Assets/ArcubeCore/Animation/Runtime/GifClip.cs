#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Arcube.Animation
{
    [CreateAssetMenu(fileName = "GifClip", menuName = "AnimationClip/GifClip")]
    public class GifClip : SpriteAnimationClip
    {
        public Object gifFile;
        public string texturePath;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(texturePath)) texturePath = AssetDatabase.GetAssetPath(gifFile);
        }
#endif
    }
}