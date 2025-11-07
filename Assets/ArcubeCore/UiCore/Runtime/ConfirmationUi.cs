using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    public enum ConfirmType
    {
        Confirm,
        Close,
        Deny,
        ConfirmAndDoNotShow,
        DenyAndDoNotShow,
    }

    public class ConfirmationUi : Ui
    {
        private static ConfirmationUi _instance;

        [SerializeField] private ButtonText b_confirm;
        [SerializeField] private ButtonText b_deny;
        [SerializeField] private ButtonImage b_close;

        [SerializeField] private TMP_Text t_title;
        [SerializeField] private TMP_Text t_body;
        [SerializeField] private GameObject doNotShowAgainObject;
        protected override void Reset()
        {
            t_title = gameObject.FindObject<TMP_Text>("t_title");
            t_body = gameObject.FindObject<TMP_Text>("t_body");
            b_confirm = gameObject.FindObject<ButtonText>("b_confirm");
            b_deny = gameObject.FindObject<ButtonText>("b_deny");
            b_close = gameObject.FindObject<ButtonImage>("b_close");

            deactivateOnClose = true;
            closable = false;
        }

        public override Task Register()
        {
            _instance = this;
            return base.Register();
        }

        public static async Task<ConfirmType> ShowMessage(string title, string body, string confirmText,
            string cancelText, bool closable = false, bool doNotShowAgain = false)
        {
            try
            {
                var result = ConfirmType.Close;

                _instance.Open();

                _instance.t_title.text = title;
                _instance.t_body.text = body;

                _instance.GetComponentInChildren<LayoutGroup>().enabled = false;
                await Awaitable.WaitForSecondsAsync(0.01f);
                _instance.GetComponentInChildren<LayoutGroup>().enabled = true;
                
                _instance.doNotShowAgainObject.SetActive(doNotShowAgain);
                var toggleButton = _instance.doNotShowAgainObject.GetComponentInChildren<ToggleButton>();

                var b_confirm = _instance.b_confirm;
                b_confirm.OnClick.RemoveAllListeners();
                
                b_confirm.OnClick.AddListener(() =>
                {
                    result = doNotShowAgain && toggleButton.IsOn ? ConfirmType.ConfirmAndDoNotShow : ConfirmType.Confirm;
                    _instance.Close();
                });

                var b_deny = _instance.b_deny;
                b_deny.OnClick.RemoveAllListeners();

                b_deny.gameObject.SetActive(true);
                b_deny.OnClick.AddListener(() =>
                {
                    result = doNotShowAgain && toggleButton.IsOn ? ConfirmType.DenyAndDoNotShow : ConfirmType.Deny;
                    _instance.Close();
                });

                var b_close = _instance.b_close;
                _instance.b_close.gameObject.SetActive(closable);
                b_close.OnClick.RemoveAllListeners();
                b_close.OnClick.AddListener(() =>
                {
                    result = ConfirmType.Close;
                    _instance.Close();
                });
                

                while (_instance.State != UiState.Closed)
                {
                    await Task.Yield();
                    _instance.destroyCancellationToken.ThrowIfCancellationRequested();
                }
                
                return result;
            }
            catch (Exception e)
            {
                Log.AddException(e);
                return ConfirmType.Close;
            }
        }

        public static async Task ShowMessage(string title, string body, string confirmText)
        {
            try
            {
                if (!_instance) return;

                _instance.Open();

                _instance.b_deny.gameObject.SetActive(false);
                _instance.doNotShowAgainObject.SetActive(false);

                _instance.t_title.text = title;
                _instance.t_body.text = body;

                var confirm = _instance.b_confirm;
                confirm.OnClick.RemoveAllListeners();
                
                confirm.OnClick.AddListener(() =>
                {
                    _instance.Close();
                });

                _instance.b_close.gameObject.SetActive(false);
                
                _instance.GetComponentInChildren<LayoutGroup>().enabled = false;
                await Awaitable.WaitForSecondsAsync(0.01f);
                _instance.GetComponentInChildren<LayoutGroup>().enabled = true;

                while (_instance.State != UiState.Closed && !_instance.destroyCancellationToken.IsCancellationRequested)
                {
                    await Task.Yield();
                    _instance.destroyCancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch(Exception e)
            {
                Log.AddException(e);
            }
        }
    }
}