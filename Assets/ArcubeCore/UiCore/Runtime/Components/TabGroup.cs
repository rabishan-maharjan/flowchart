using UnityEngine;
using UnityEngine.Events;

namespace Arcube.UiManagement
{
    public class TabGroup : MonoBehaviour
    {
        public int Value {  get; private set; }
        [SerializeField] private TabSelector selectedTab;
        private TabSelector _prevTab;
        internal TabSelector SelectedItem
        {
            get => selectedTab;
            set => SetSelectedTab(value);
        }

        [SerializeField] private TabSelector[] tabs;
        private void Reset()
        {
            tabs = GetComponentsInChildren<TabSelector>();
        }

        private void OnValidate()
        {
            if (selectedTab) SetSelectedTab(selectedTab);
        }

        public UnityEvent<int> OnTabSwitched;
        private void SetSelectedTab(TabSelector tab)
        {
            if (tab == _prevTab) return;
            if (_prevTab != null) 
            {
                _prevTab.RemoveHighlight();
                _prevTab.tabPanel.Disable();
            }

            selectedTab = tab;
            OnTabSwitched?.Invoke(tab.transform.GetSiblingIndex());
            Value = selectedTab.transform.GetSiblingIndex();
            selectedTab.Highlight();

            selectedTab.tabPanel.Enable();

            _prevTab = selectedTab;
        }

        private void Start() => SetSelectedTab(selectedTab);

        public virtual void Set(int tab)
        {
            Value = tab;
            SetSelectedTab(tabs[tab]);
        }

        public void SetTabs(string[] tabs, TabPanel panel)
        {
        }

        public void SetTabs(Sprite[] tabs, TabPanel panel)
        {
        }
    }
}