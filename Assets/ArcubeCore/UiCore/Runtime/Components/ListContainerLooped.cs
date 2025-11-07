using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    public class ListContainerLooped : ListContainer
    {
        [SerializeField] private ScrollRect scrollRect;
        private RectTransform _content;
        protected override void Reset()
        {
            base.Reset();
            scrollRect = GetComponent<ScrollRect>();
        }

        private void Start() => scrollRect.onValueChanged.AddListener(CheckLoop);

        private float _resetThreshold;
        private float _itemWidth;
        private int _itemCount;
        [SerializeField] private int visibleCopies = 3;

        protected override void SetUi()
        {
            if (isStatic && Initialized) return;
            if (Items == null) return;

            ClearList();
            
            // Duplicate for looping
            _itemCount = 0;
            _content = (RectTransform)panelItemPrefab.transform.parent;
            var itemArray = new List<object>();
            foreach (var item in Items)
            {
                var obj = Instantiate(panelItemPrefab, _content);
                obj.Set(item);
                Objects.Add(obj);
                itemArray.Add(item);
                _itemCount++;
            }
            
            // Instantiate duplicates after
            for (var i = 0; i < visibleCopies; i++)
            {
                CreateItem(itemArray[i % _itemCount]);
            }

            Canvas.ForceUpdateCanvases(); // Layout update

            _itemWidth = panelItemPrefab.GetComponent<RectTransform>().rect.width;

            // Move to first real item (centered)
            var centerIndex = visibleCopies;
            var offset = -centerIndex * _itemWidth;
            _content.anchoredPosition = new Vector2(offset, _content.anchoredPosition.y);
            
            _itemWidth = ((RectTransform)panelItemPrefab.transform).sizeDelta.x;
            _resetThreshold = _itemWidth * _itemCount;

            Initialized = true;
        }
        
        private void CheckLoop(Vector2 value)
        {
            var positionX = _content.anchoredPosition.x;
            var totalWidth = _itemWidth * _itemCount;

            if (positionX > -_itemWidth * 0.5f) // Scrolled too far left
            {
                _content.anchoredPosition -= new Vector2(totalWidth, 0);
            }
            else if (positionX < -(_itemWidth * (_itemCount + visibleCopies - 0.5f))) // Too far right
            {
                _content.anchoredPosition += new Vector2(totalWidth, 0);
            }
        }

        private void CreateItem(object item)
        {
            var obj = Instantiate(panelItemPrefab, _content);
            obj.Set(item);
            Objects.Add(obj);
        }
    }
}