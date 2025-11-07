using Arcube.Animation;

namespace Arcube.UiManagement
{
    public class UiAnimator : DOTweenAnimator
    {
        private Ui ui;
        private void Start()
        {
            ui = GetComponentInParent<Ui>();

            GetClip("Open").OnComplete.AddListener(ui.OnOpen);
            GetClip("Close").OnComplete.AddListener(ui.OnClose);
        }

        public void PlayOpen()
        {
            Play(GetClip("Open"));
        }

        public void PlayClose()
        {
            Play(GetClip("Close"));
        }
    }
}