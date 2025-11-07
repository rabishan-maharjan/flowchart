using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arcube
{
    public class ButtonDelayed : Button
    {
        [SerializeField] private float delay = 0.2f;
        protected override void Start()
        {
            base.Start();
            onClick.AddListener(() =>
            {
                Delayer.Delay(delay, () => onClickDelayed?.Invoke());
            });
        }

        public UnityEvent onClickDelayed;
        public void AddListners(UnityAction action) { onClickDelayed.AddListener(action); }

        public void RemoveListners(UnityAction action) { onClickDelayed.RemoveListener(action); }

        public void RemoveAllListners() { onClickDelayed.RemoveAllListeners(); }
    }
}