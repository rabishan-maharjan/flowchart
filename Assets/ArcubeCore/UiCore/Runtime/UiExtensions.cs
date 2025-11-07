using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube.UiManagement
{
    public static class UiExtensions
    {
        public static Vector2 GetSize(this RectTransform rt)
        {
            return new Vector2(rt.rect.width, rt.rect.height);
        }

        public static void SnapTo(this ScrollRect scrollRect, RectTransform target, RectTransform contentPanel)
        {
            Canvas.ForceUpdateCanvases();

            contentPanel.anchoredPosition =
                    (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                    - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }

        public static IEnumerator LerpToPosition(this ScrollRect scrollRect, RectTransform target, RectTransform contentPanel, Vector2 lerpDir, Vector2 targetOffsetPos)
        {
            var startPos = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position);

            if (lerpDir.y == 0)
                startPos.y = 0;
            else if (lerpDir.x == 0)
                startPos.x = 0;

            Vector2 _lerpTo = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
            _lerpTo += targetOffsetPos;
            
            Canvas.ForceUpdateCanvases();            

            float decelerate = 0;

            while (decelerate < 1f)
            {
                decelerate += 1 * Time.deltaTime;                
                contentPanel.anchoredPosition = Vector2.Lerp(startPos, _lerpTo, decelerate);
                yield return null;
            }
        }
    }
}