using UnityEngine;
using UnityEngine.UI;

namespace Arcube
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image progressImage;
        private void Reset() => progressImage = transform.FindObject<Image>("i_progress");

        public void Set(float value)
        {
            if (progressImage != null) progressImage.fillAmount = value;
        }
    }
}