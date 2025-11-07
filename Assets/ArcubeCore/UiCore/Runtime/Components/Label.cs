using TMPro;
using UnityEngine;

namespace Arcube.UiManagement
{
    [AddComponentMenu("ArcubeUI/Label")]
    public class Label : TMP_Text
    {
        public string Text
        {
            get => text;
            set
            {
                text = value;
            }
        }
    }
}