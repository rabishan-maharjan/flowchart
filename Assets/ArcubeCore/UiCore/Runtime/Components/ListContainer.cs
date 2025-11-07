using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    public abstract class PanelItem : ButtonText
    {
        public virtual void Set<T>(T item)
        {
            gameObject.SetActive(true);
        }
    }

    public class ListContainer : Panel
    {
        [SerializeField] private Transform highlight;
        [SerializeField] protected PanelItem panelItemPrefab;
        //private static Vector2 lastPosition;
        //private static PanelItem lastSelected;
        protected virtual void Reset()
        {
            panelItemPrefab = GetComponentInChildren<PanelItem>(true);
        }

        [SerializeField] private Transform firstEmptyItem;
        [SerializeField] private Transform lastEmptyItem;
        protected IEnumerable Items;
        public ListContainer Set(IEnumerable listItems)
        {
            Items = listItems;

            return this;
        }

        public bool isStatic = true;
        protected bool Initialized;
        public List<PanelItem> Objects { get; private set; } = new();

        protected override void SetUi()
        {
            if (isStatic && Initialized) return;
            if (Items == null) return;

            ClearList();

            var content = panelItemPrefab.transform.parent;
            foreach (var item in Items)
            {
                var obj = Instantiate(panelItemPrefab, content);
                obj.Set(item);
                obj.name = $"tg_{item}";
                Objects.Add(obj);
            }

            if (lastEmptyItem) lastEmptyItem.SetAsLastSibling();
            if (firstEmptyItem) firstEmptyItem.SetAsFirstSibling();

            Initialized = true;
        }

        public void ClearList()
        {
            for (var index = Objects.Count - 1; index >= 0; index--)
            {
                var item = Objects[index];
                Destroy(item.gameObject);
            }

            Objects.Clear();
        }

        public override void Disable()
        {
            base.Disable();

            if (TryGetComponent<ScrollRect>(out var scroll))
            {
                //lastPosition.y = scroll.verticalNormalizedPosition;
                //lastPosition.x = scroll.horizontalNormalizedPosition;
            }
        }

        public void Select(PanelItem panel)
        {
            if (!highlight) return;
            highlight.SetParent(panel.transform);
            highlight.SetSiblingIndex(0);
            var rt = (RectTransform)highlight;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }

        public T CreateItem<T>() => Instantiate(panelItemPrefab, transform).GetComponent<T>();
    }
}