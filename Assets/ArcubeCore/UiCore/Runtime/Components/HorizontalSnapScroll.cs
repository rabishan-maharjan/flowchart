using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    public class HorizontalSnapScroll : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public RectTransform content;
        public RectTransform viewport;
        public Button leftArrow;
        public Button rightArrow;

        private int _currentIndex;
        private int _totalItems;
        private float _itemWidth;
        private void Start()
        {
            _totalItems = content.childCount;

            if (_totalItems < 2) return;

            // Assumes all items are same width
            _itemWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width;

            if(leftArrow)
                leftArrow.onClick.AddListener(ScrollLeft);
            if(rightArrow)
                rightArrow.onClick.AddListener(ScrollRight);

            UpdateArrowVisibility();
        }

        private void ScrollLeft()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                SnapToIndex(_currentIndex);
            }
        }

        private void ScrollRight()
        {
            if (_currentIndex < _totalItems - 1)
            {
                _currentIndex++;
                SnapToIndex(_currentIndex);
            }
        }

        private void SnapToIndex(int index)
        {
            var targetX = index * _itemWidth;
            var normalizedPosition = targetX / (content.rect.width - viewport.rect.width);
            StartCoroutine(SmoothScroll(normalizedPosition));

            UpdateArrowVisibility();
        }

        private void UpdateArrowVisibility()
        {
            leftArrow.interactable = _currentIndex > 0;
            rightArrow.interactable = _currentIndex < _totalItems - 1;
        }
        
        private IEnumerator SmoothScroll(float target)
        {
            var duration = 0.2f;
            var start = scrollRect.horizontalNormalizedPosition;
            var time = 0f;

            while (time < duration)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            scrollRect.horizontalNormalizedPosition = target;
        }
    }
}