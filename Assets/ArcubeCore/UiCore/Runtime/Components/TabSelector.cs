using UnityEngine;

namespace Arcube.UiManagement
{
    public class TabSelector : ButtonText
    {
        public TabPanel tabPanel;
        public RectTransform highlight;
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            highlight = transform.Find("highlight") as RectTransform;
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            OnClick.AddListener(() =>
            {
                var list = GetComponentInParent<TabGroup>();
                list.SelectedItem = this;
            });
        }

        internal void Highlight()
        {
            if (highlight) highlight.gameObject.SetActive(true);
        }

        internal void RemoveHighlight()
        {
            if(highlight) highlight.gameObject.SetActive(false);
        }
    }
}