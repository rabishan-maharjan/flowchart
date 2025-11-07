using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arcube.Animation
{
    [System.Serializable]
    public class TweenBuilder
    {
        public static string[] TweenTypes;
        public static string[] TweenCommands;

        public TweenBuilder()
        {
            TweenTypes = Extensions.GetMethodsInClass<DoTweenAnimationHandler>();
            TweenCommands = Extensions.GetFields<DoTweenConstants>();
        }

        [System.Serializable]
        public class TweenField
        {
            public string command;
            public string value;

            public float AsInt => int.Parse(value);
            public float AsFloat => float.Parse(value);
            public string AsString => value;
            public Vector2 AsVector2 => Utils.TryParseVector2(value, out Vector2 result) ? result : Vector2.positiveInfinity;
            public Vector3 AsVector3 => Utils.TryParseVector3(value, out Vector3 result) ? result : Vector3.positiveInfinity;
            public GameObject AsObject => Extensions.FindObjectFromInstanceID(int.Parse(value)) as GameObject;
        }

        [System.Serializable]
        public class Tween
        {
            //public string name;
            public List<TweenField> tweenFields = new List<TweenField>();
            public TweenField this[string key] => tweenFields.FirstOrDefault((p) => p.command == key);
        }

        [System.Serializable]
        public class Animation
        {
            //public string name;
            public List<Tween> tweens = new List<Tween>();
            public List<TweenField> tweenFields = new List<TweenField>();
            public TweenField this[string key] => tweenFields.FirstOrDefault((p) => p.command == key);
        }

        public List<Animation> animation = new List<Animation>();
        //public Animation this[string key] => animation.FirstOrDefault((p) => p.name == key);

        //public JSONNode GetJsonNode(string key) {
        //    JSONNode node = JSON.Parse("[]");
        //    foreach (var anim in animation) {
        //        if (anim.name == key) {
        //            foreach(var tweenField in anim.tweenFields) {
        //                node.Add(tweenField.key, tweenField.param);
        //            }
        //            foreach(var tween in anim.tweens) {
        //                JSONNode tweens = JSON.Parse("{}");
        //                foreach (var tweenField in tween.tweenFields) {
        //                    tweens.Add(tweenField.key, tweenField.param);
        //                }
        //                node.Add(tweens);
        //            }
        //            break;
        //        }
        //    }

        //    return node;
        //}

        public string GetJsonString()
        {
            string s = "[";
            foreach (var anim in animation)
            {
                foreach (var tween in anim.tweens)
                {
                    string s1 = "{";
                    foreach (var tweenField in tween.tweenFields)
                    {
                        s1 = $"{s1} \"{ tweenField.command}\":\"{tweenField.value}\",";
                    }
                    s = $"{s}{s1.Remove(s1.Length - 1)}}}";
                }

                string s2 = "";
                foreach (var tweenField in anim.tweenFields)
                {
                    if (tweenField.value == "") s2 = $"{s2}\"{ tweenField.command}\",";
                    else s2 = $"{s2}\"{ tweenField.command}\":\"{tweenField.value}\",";
                }
                if (s2.Length == 0) return $"{s}]";
                s = $"{s},{s2.Remove(s2.Length - 1)}]";
            }

            return s;
        }
    }
}