using System;
using UnityEditor;
using UnityEngine;

namespace Arcube.Animation.EditorExtension
{
    [CustomPropertyDrawer(typeof(TweenBuilder.TweenField))]
    public class TweenBuilderEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;

            var keyRect = new Rect(position.x, position.y, 200, position.height);
            var gap = (keyRect.width) + 5;
            var paramRect = new Rect(position.x + gap, position.y, position.width - gap, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            var key = property.FindPropertyRelative("command");
            if (string.IsNullOrEmpty(key.stringValue))
            {
                key.stringValue = TweenBuilder.TweenCommands[0];
            }
            var keyIndex = Mathf.Max(0, Array.IndexOf(TweenBuilder.TweenCommands, key.stringValue));
            keyIndex = EditorGUI.Popup(keyRect, keyIndex, TweenBuilder.TweenCommands);
            key.stringValue = TweenBuilder.TweenCommands[keyIndex];

            var param = property.FindPropertyRelative("value");
            if (key.stringValue == DoTweenConstants.TweenType)
            {
                if (string.IsNullOrEmpty(param.stringValue))
                {
                    param.stringValue = TweenBuilder.TweenTypes[0];
                }
                int index = Mathf.Max(0, Array.IndexOf(TweenBuilder.TweenTypes, param.stringValue));
                index = EditorGUI.Popup(paramRect, index, TweenBuilder.TweenTypes);
                param.stringValue = TweenBuilder.TweenTypes[index];
            }
            else if (key.stringValue == DoTweenConstants.EaseType)
            {
                if (string.IsNullOrEmpty(param.stringValue))
                {
                    param.stringValue = "InOutElastic";
                }
                if (Enum.TryParse(param.stringValue, out DG.Tweening.Ease value))
                {
                    value = (DG.Tweening.Ease)EditorGUI.EnumPopup(paramRect, value);
                    param.stringValue = value.ToString();
                }
                else
                {
                    value = DG.Tweening.Ease.InOutElastic;
                    value = (DG.Tweening.Ease)EditorGUI.EnumPopup(paramRect, value);
                    param.stringValue = value.ToString();
                }
            }
            else if (key.stringValue == DoTweenConstants.LoopType)
            {
                if (string.IsNullOrEmpty(param.stringValue))
                {
                    param.stringValue = "Yoyo";
                }
                if (Enum.TryParse(param.stringValue, out DG.Tweening.LoopType value))
                {
                    value = (DG.Tweening.LoopType)EditorGUI.EnumPopup(paramRect, value);
                    param.stringValue = value.ToString();
                }
                else
                {
                    value = DG.Tweening.LoopType.Yoyo;
                    value = (DG.Tweening.LoopType)EditorGUI.EnumPopup(paramRect, value);
                    param.stringValue = value.ToString();
                }
            }
            else if (key.stringValue == DoTweenConstants.Target)
            {
                UnityEngine.Object obj = null;
                if (int.TryParse(param.stringValue, out int id))
                {
                    obj = Extensions.FindObjectFromInstanceID(id);
                }
                obj = EditorGUI.ObjectField(paramRect, obj, typeof(GameObject), true);
                if (obj != null)
                {
                    param.stringValue = obj.GetInstanceID().ToString();
                }
            }
            else
            {
                EditorGUI.PropertyField(paramRect, param, GUIContent.none);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public string GetDefaultToValue(string tweenType) => tweenType switch
        {
            "FadeCanvas" => "1",
            "FadeImage" => "1",
            "FadeText" => "1",
            "MoveRectTransform"=>@"{""x"":0,""y"":0}",
            "PunchMoveRectTransform" => @"{""x"":0,""y"":0}",
            _ => "",
        };
    }
}