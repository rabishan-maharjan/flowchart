using UnityEngine;
using UnityEngine.UI;

namespace Arcube.Utility
{
    [RequireComponent((typeof(Image)))]
    public class RandomColor : MonoBehaviour
    {
        [SerializeField] private Color[] colors;
        [Range(0, 1)] [SerializeField] private float brightness = 1;
        [Range(0, 1)] [SerializeField] private float saturation = 0.9f;
        private void OnEnable()
        {
            var color = colors.Length > 0
                ? colors[Random.Range(0, colors.Length)]
                : Color.HSVToRGB(Random.value, saturation, brightness);
            ;
            GetComponent<Image>().color = color;
        }
    }
}