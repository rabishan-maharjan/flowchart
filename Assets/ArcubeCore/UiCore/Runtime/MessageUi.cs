using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Arcube.UiManagement
{
    public class MessageUi : Ui
    {
        private static MessageUi _instance;
        [SerializeField] private TMP_Text text = null;
        protected override void Reset() 
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        public override Task Register()
        {
            _instance = this;
            return base.Register();
        }

        private void Open(string message, float delay = 3)
        {
            text.text = message;

            Open();

            StartCoroutine(WaitForClose(delay));
        }

        public static void Show(string message, float delay = 3)
        {
            if (_instance != null) _instance.Open(message, delay);
        }

        private IEnumerator WaitForClose(float delay)
        {
            yield return new WaitForSeconds(delay);

            Close();
        }
    }
}