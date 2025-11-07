using UnityEngine;

namespace Arcube.UiManagement
{
    /// <summary>
    /// static panel
    /// </summary>
    public abstract class Panel : MonoBehaviour
    {
        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        public virtual void Enable()
        {
            SetUi();
            gameObject.SetActive(true);
        }

        protected virtual void SetUi() { }
    }

    /// <summary>
    /// dynamic panel
    /// </summary>
    public class TabPanel : Panel
    {
    }
}