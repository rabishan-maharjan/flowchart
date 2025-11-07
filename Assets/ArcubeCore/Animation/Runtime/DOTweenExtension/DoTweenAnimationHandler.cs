using DG.Tweening;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube.Animation
{
    public class DoTweenAnimationHandler
    {
        public static Tweener UiSlide(JSONNode node, Transform obj, Transform path = null)
        {
            var rt = obj.GetComponent<RectTransform>();
            var size = new Vector2(rt.rect.width, rt.rect.height);
            var to = node[DoTweenConstants.To].ReadVector2();
            return rt.DOAnchorPos(new Vector2(to.x * size.x, to.y * size.y), node[DoTweenConstants.Duration].AsFloat);
        }

        //public static Tweener FadeCanvas(TweenBuilder.Tween tween, Transform obj, Transform path = null) => obj.GetComponent<CanvasGroup>().DOFade(tween[DoTweenConstants.To].AsFloat, tween[DoTweenConstants.Duration].AsFloat);
        public static Tweener FadeCanvas(JSONNode node, Transform obj, Transform path = null)
        {
            var v = obj.GetComponent<CanvasGroup>().DOFade(node[DoTweenConstants.To].AsFloat, node[DoTweenConstants.Duration].AsFloat);
            if (node[DoTweenConstants.From])
            {
                v.From(node[DoTweenConstants.From].AsFloat);
            }
            return v;
        }

        public static Tweener FadeImage(JSONNode node, Transform obj, Transform path = null)
        {
            var v =  obj.GetComponentInChildren<Image>()
                .DOFade(node[DoTweenConstants.To].AsFloat, node[DoTweenConstants.Duration].AsFloat);
            
            if (node[DoTweenConstants.From])
            {
                v.From(node[DoTweenConstants.From].AsFloat);
            }
            
            return v;
        }

        public static Tweener FadeText(JSONNode node, Transform obj, Transform path = null)
        {
            var to = node[DoTweenConstants.To].AsFloat;
            var text = obj.GetComponentInChildren<TMP_Text>();
            var v = text.DOFade(to, node[DoTweenConstants.Duration].AsFloat);
            if (node[DoTweenConstants.From])
            {
                var from = node[DoTweenConstants.From].AsFloat;
                v.From(from);
            }
            
            return v;
        }

        public static Tweener MoveRectTransform(JSONNode node, Transform obj, Transform path = null)
        {
            var v = obj.GetComponent<RectTransform>().DOAnchorPos(node[DoTweenConstants.To].ReadVector2(),
                node[DoTweenConstants.Duration].AsFloat);
            if (!node[DoTweenConstants.From].IsNull)
            {
                var from = node[DoTweenConstants.From].ReadVector2();
                v.From(from);
            }
            
            return v;
        }

        public static Tweener MovePath(JSONNode node, Transform obj, Transform path = null) => obj.DOMove(path.position, node[DoTweenConstants.Duration].AsFloat);
        public static Tweener MovePathLocal(JSONNode node, Transform obj, Transform path = null) => obj.DOLocalMove(path.localPosition, node[DoTweenConstants.Duration].AsFloat);
        public static Tweener MoveLocal(JSONNode node, Transform obj, Transform path = null) => obj.DOLocalMove(node[DoTweenConstants.To].ReadVector3(), node[DoTweenConstants.Duration].AsFloat);
        public static Tweener PunchMoveRectTransform(JSONNode node, Transform obj, Transform path = null) => obj.GetComponent<RectTransform>().DOPunchAnchorPos(node[DoTweenConstants.To].ReadVector2(), node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Vibrato], node[DoTweenConstants.Elasticity].AsFloat);
        public static Tweener PunchMove(JSONNode node, Transform obj, Transform path = null) => obj.DOPunchPosition(path ? path.position : node[DoTweenConstants.To].ReadVector3(), node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Vibrato], node[DoTweenConstants.Elasticity].AsFloat);

        public static Tweener ColorMaterial(JSONNode node, Transform obj, Transform path = null) => obj.GetComponent<Renderer>().material.DOColor(DOTweenWrapper.GetColor(node[DoTweenConstants.To].Value), node[DoTweenConstants.Field].Value, node[DoTweenConstants.Duration].AsFloat);
        public static Tweener ColorText(JSONNode node, Transform obj, Transform path = null)
        {
            var to = DOTweenWrapper.GetColor(node[DoTweenConstants.To].Value);
            var text = obj.GetComponentInChildren<TMP_Text>();
            to.a = text.color.a;
            var v = text.DOColor(to, node[DoTweenConstants.Duration].AsFloat);
            if (node[DoTweenConstants.From])
            {
                var from = DOTweenWrapper.GetColor(node[DoTweenConstants.From].Value);
                from.a = text.color.a;
                v.From(from);
            }

            return v;
        }

        public static Tweener ColorImage(JSONNode node, Transform obj, Transform path = null)
        {
            var to = DOTweenWrapper.GetColor(node[DoTweenConstants.To].Value);
            var image = obj.GetComponentInChildren<Image>();
            to.a = image.color.a;
            var v = image.DOColor(to, node[DoTweenConstants.Duration].AsFloat);
            if (node[DoTweenConstants.From])
            {
                var from = DOTweenWrapper.GetColor(node[DoTweenConstants.From].Value);
                from.a = image.color.a;
                v.From(from);
            }

            return v;
        }

        public static Tweener ColorSprite(JSONNode node, Transform obj, Transform path = null) => obj.GetComponentInChildren<SpriteRenderer>().DOColor(DOTweenWrapper.GetColor(node[DoTweenConstants.To].Value), node[DoTweenConstants.Duration].AsFloat);
        public static Tweener ColorInputField(JSONNode node, Transform obj, Transform path = null) => obj.GetComponent<TMP_InputField>().image.DOColor(DOTweenWrapper.GetColor(node[DoTweenConstants.To].Value), node[DoTweenConstants.Duration].AsFloat);

        public static Tweener ScaleUniform(JSONNode node, Transform obj, Transform path = null)
        {
            var v = obj.DOScale(path ? path.localScale : Vector3.one * node[DoTweenConstants.To].AsFloat, node[DoTweenConstants.Duration].AsFloat);
            if (node[DoTweenConstants.From])
            {
                v.From(node[DoTweenConstants.From].AsFloat);
            }
            return v;
        }

        public static Tweener PunchScaleUniform(JSONNode node, Transform obj, Transform path = null) => obj.DOPunchScale(Vector3.one * node[DoTweenConstants.To].AsFloat, node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Vibrato].AsInt, node[DoTweenConstants.Elasticity].AsFloat);
        
        public static Tweener Rotate(JSONNode node, Transform obj, Transform path = null)
        {
            var v = obj.DOLocalRotate(path ? path.eulerAngles : node[DoTweenConstants.To].ReadVector3(), node[DoTweenConstants.Duration].AsFloat);
            if (!node[DoTweenConstants.From].IsNull)
            {
                v.From(node[DoTweenConstants.From].ReadVector3());
            }
            return v;
        }

        public static Tweener ShakePosition(JSONNode node, Transform obj, Transform path = null) => obj.DOShakePosition(node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Strength].ReadVector3(), node[DoTweenConstants.Vibrato], node[DoTweenConstants.Randomness].AsFloat);
        public static Tweener ShakeRectPosition(JSONNode node, Transform obj, Transform path = null) => obj.GetComponent<RectTransform>().DOShakeAnchorPos(node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Strength].ReadVector3(), node[DoTweenConstants.Vibrato], node[DoTweenConstants.Randomness].AsFloat);
        public static Tweener ShakeRotation(JSONNode node, Transform obj, Transform path = null) => obj.DOShakeRotation(node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Strength].ReadVector3(), node[DoTweenConstants.Vibrato].AsInt, node[DoTweenConstants.Randomness].AsFloat);
        public static Tweener ShakeScale(JSONNode node, Transform obj, Transform path = null) => obj.DOShakeScale(node[DoTweenConstants.Duration].AsFloat, node[DoTweenConstants.Strength].ReadVector3(), node[DoTweenConstants.Vibrato], node[DoTweenConstants.Randomness].AsFloat);

        //public static Tweener Text(JSONNode node, Transform obj, Transform path = null) => obj.GetComponent<Text>().DOText(node[DoTweenConstants.To].Value, node[DoTweenConstants.Duration].AsFloat);

        public static Tweener Fill(JSONNode node, Transform obj, Transform path = null) => obj.GetComponent<Image>().DOFillAmount(node[DoTweenConstants.To].AsFloat, node[DoTweenConstants.Duration].AsFloat);

        public static Tweener Wait(JSONNode node, Transform obj, Transform path = null)
        {
            int f = 0;
            return DOTween.To(() => f, x => f = x, 1, node[DoTweenConstants.Duration].AsFloat);
        }
    }
}