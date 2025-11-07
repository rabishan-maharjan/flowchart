using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    [RequireComponent(typeof(ButtonImage))]
    public class ScrollArrowIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float scrollStep = 0.1f;
        public void OnPointerDown(PointerEventData eventData)
        {
            StartCoroutine(Scroll());
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
        }

        private const int ScrollFactor = 100;
        private IEnumerator Scroll()
        {
            while (true)
            {
                scrollRect.verticalNormalizedPosition -= scrollStep * ScrollFactor * Time.deltaTime / scrollRect.content.rect.height;
                yield return null;
            }
        }
    }
}