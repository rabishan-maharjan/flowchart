using DG.Tweening;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arcube.Animation
{
    public static class DOTweenWrapper
    {
        public delegate Tweener TweenNode(JSONNode node, Transform obj, Transform path = null);
        public delegate Tweener TweenObject(TweenBuilder.Tween tween, Transform obj, Transform path = null);

        public static bool initialized = false;
        public static Dictionary<string, TweenNode> functions;

        public static Sequence PlaySequence(TweenBuilder.Animation animation, Transform obj)
        {
            var s = DOTween.Sequence();
            foreach (var tween in animation.tweens)
            {
                var tweener = GetTweener(tween, obj);
                s.Insert(tween[DoTweenConstants.Delay] != null ? tween[DoTweenConstants.Delay].AsFloat : 0, tweener);
            }

            return s;
        }
        public static Sequence PlaySequence(JSONNode node, Transform obj)
        {
            var sequence = DOTween.Sequence();
            foreach (JSONNode n in node)
            {
                var targets = GetTargets(n, obj);
                foreach (var target in targets)
                {
                    var tweener = GetTweener(n, obj, target);
                    sequence.Append(tweener);
                }
            }

            AddSequnceConditions(node, obj, sequence);

            return sequence;
        }

        public static Sequence PlayAll(JSONNode node, Transform obj)
        {
            var sequence = DOTween.Sequence();
            var tweenNodes = node["Tweens"].IsArray ? node["Tweens"] : node;
            foreach (JSONNode n in tweenNodes)
            {
                var targets = GetTargets(n, obj);
                foreach (var target in targets)
                {
                    var tweener = GetTweener(n, obj, target);
                    if (tweener != null) sequence.Insert(n[DoTweenConstants.Delay] ? n[DoTweenConstants.Delay].AsFloat : 0, tweener);
                }
            }

            AddSequnceConditions(node, obj, sequence);

            return sequence;
        }

        private static void AddSequnceConditions(JSONNode node, Transform obj, Sequence sequence)
        {
            if (node[DoTweenConstants.LoopType])
            {
                sequence.SetLoops(node[DoTweenConstants.LoopCount] ? node[DoTweenConstants.LoopCount].AsInt : -1, (LoopType)Enum.Parse(typeof(LoopType), node[DoTweenConstants.LoopType].Value));
            }

            if (node[DoTweenConstants.EaseType]) sequence.SetEase((Ease)Enum.Parse(typeof(Ease), node[DoTweenConstants.EaseType].Value));

            if (node[DoTweenConstants.Relative]) sequence.SetRelative();

            if (node[DoTweenConstants.OnComplete])
            {
                sequence.OnComplete(() =>
                {
                    obj.SendMessage(node[DoTweenConstants.OnComplete].Value, SendMessageOptions.DontRequireReceiver); //always check for event complete in main caller object.
                });
            }
        }

        public static Transform[] GetTargets(JSONNode node, Transform obj)
        {
            var targets = new List<Transform>(); //determine the target transform to animate
            if (node[DoTweenConstants.Target])
            {
                if (node[DoTweenConstants.Target].Value == "parent")
                {
                    targets.Add(obj.parent);
                }
                else if (node[DoTweenConstants.Target].Value == "children")
                {
                    var type = Type.GetType(node[DoTweenConstants.ChildType].Value);
                    foreach (var t in obj.GetComponentsInChildren(type))
                    {
                        targets.Add(t.transform);
                    }
                }
                else
                {
                    var target = obj.FindObject<Transform>(node[DoTweenConstants.Target].Value);
                    if (target != null)
                    {
                        targets.Add(target);
                    }
                    else
                    {
                        Log.Add(()=> $"Can't find target object {node[DoTweenConstants.Target].Value}");
                    }
                }
            }
            else
            {
                targets.Add(obj);
            }

            return targets.ToArray();
        }

        private static Tweener GetTweener(JSONNode node, Transform obj, Transform target, Transform path = null)
        {
            if (node[DoTweenConstants.TweenType] == null) return null;

            var method = (TweenNode)Delegate.CreateDelegate(typeof(TweenNode), typeof(DoTweenAnimationHandler).GetMethod(node[DoTweenConstants.TweenType]));
            var tweener = method.Invoke(node, target, path);

            if (tweener == null) return null;

            if (node[DoTweenConstants.LoopType])
            {
                tweener.SetLoops(node[DoTweenConstants.LoopCount].IsNumber ? node[DoTweenConstants.LoopCount].AsInt : -1, (LoopType)Enum.Parse(typeof(LoopType), node[DoTweenConstants.LoopType].Value));
            }

            if (node[DoTweenConstants.EaseType]) tweener.SetEase((Ease)Enum.Parse(typeof(Ease), node[DoTweenConstants.EaseType].Value));

            if (node[DoTweenConstants.Relative]) tweener.SetRelative();

            return tweener;
        }

        private static Tweener GetTweener(TweenBuilder.Tween tween, Transform obj, Transform path = null)
        {
            if (tween[DoTweenConstants.Target] != null)
            {
                obj = tween[DoTweenConstants.Target].AsObject.transform;
            }

            var method = (TweenObject)Delegate.CreateDelegate(typeof(TweenObject), typeof(DoTweenAnimationHandler).GetMethod(tween[DoTweenConstants.TweenType].AsString));
            var tweener = method.Invoke(tween, obj, path);

            //if (node[LoopType]) tweener.SetLoops(node[LoopCount].IsNumber ? node[LoopCount].AsInt : -1, (LoopType)Enum.Parse(typeof(LoopType), node[LoopType].Value));

            //if (node[EaseType]) tweener.SetEase((Ease)Enum.Parse(typeof(Ease), node[EaseType].Value));

            //if (node[Relative]) tweener.SetRelative();

            if (tween[DoTweenConstants.OnComplete] != null)
            {
                tweener.OnComplete(() =>
                {
                    obj.SendMessage(tween[DoTweenConstants.OnComplete].AsString, SendMessageOptions.DontRequireReceiver);
                });
            }

            return tweener;
        }

        public static Color GetColor(string name) => ColorUtility.TryParseHtmlString(name, out Color color) ? color : Color.white;
    }
}