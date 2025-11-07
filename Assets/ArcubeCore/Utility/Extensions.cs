using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Arcube
{
    public static class Extensions
    {
        public static T FindObject<T>(this Transform parent, string name, bool findHidden = true) where T : class
        {
            foreach (var child in parent.GetComponentsInChildren<Transform>(findHidden))
            {
                Transform searchResult;
                if (!(searchResult = child.Find(name))) continue;
                if (searchResult.TryGetComponent(out T result))
                {
                    return result;
                }
            }

            return null;
        }

        public static bool TryFindObject<T>(this Transform parent, string name, out T result, bool findHidden = true) where T : class
        {
            foreach (var child in parent.GetComponentsInChildren<Transform>(findHidden))
            {
                Transform searchResult;
                if (!(searchResult = child.Find(name))) continue;
                if (searchResult.TryGetComponent(out result))
                {
                    return true;
                }

                Log.Add(()=> $"{name} of type {typeof(T)} not found");
            }

            result = null;
            return false;
        }

        public static T FindObject<T>(this GameObject parent, string name) where T : Component
        {
            foreach (var child in parent.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.name == name) return child;
            }

            return null;
        }

        public static T[] FindObjectsOfTag<T>(this GameObject parent, string tag) where T : Component
        {
            var results = new List<T>();
            foreach (var child in parent.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.CompareTag(tag)) results.Add(child);
            }

            return results.ToArray();
        }

        public static T[] FindObjectsOfName<T>(this GameObject parent, string name) where T : Component
        {
            var results = new List<T>();
            foreach (var child in parent.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.name == name) results.Add(child);
            }

            return results.ToArray();
        }

        public static bool TryParseVector3(string value, out Vector3 result)
        {
            var sArray = value.Split(',');
            result = Vector3.zero;

            if (!float.TryParse(sArray[0], out result.x)) return false;
            if (!float.TryParse(sArray[1], out result.y)) return false;
            if (!float.TryParse(sArray[2], out result.z)) return false;

            return true;
        }
        public static bool TryParseVector2(string value, out Vector2 result)
        {
            var sArray = value.Split(',');
            result = Vector3.zero;

            if (!float.TryParse(sArray[0], out result.x)) return false;
            if (!float.TryParse(sArray[1], out result.y)) return false;

            return true;
        }

        private static Func<int, UnityEngine.Object> m_FindObjectFromInstanceID = null;
        public static UnityEngine.Object FindObjectFromInstanceID(int aObjectID)
        {
            if (m_FindObjectFromInstanceID != null) return m_FindObjectFromInstanceID(aObjectID);
            var methodInfo = typeof(UnityEngine.Object)
                .GetMethod("FindObjectFromInstanceID",
                    BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo == null)
            {
                Log.AddError(()=> "FindObjectFromInstanceID was not found in UnityEngine.Object");
                return null;
            }

            m_FindObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(typeof(Func<int, UnityEngine.Object>), methodInfo);

            return m_FindObjectFromInstanceID(aObjectID);
        }

        public static string[] GetMethodsInClass<T>() where T : class
        {
            var methodInfos = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static);
            Array.Sort(methodInfos, (methodInfo1, methodInfo2) => string.Compare(methodInfo1.Name, methodInfo2.Name, StringComparison.Ordinal));

            var methods = methodInfos.Select(field => field.Name).ToArray();

            return methods.ToArray();
        }

        public static string[] GetFields<T>() where T : class
        {
            return typeof(T).GetFields().Select(field => field.Name).ToArray();
        }
        
        /// <summary>
        /// Checks if this RectTransform is completely inside another RectTransform.
        /// Takes into account rotation and scale of both transforms.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to check</param>
        /// <param name="other">The RectTransform to check against</param>
        /// <returns>True if this RectTransform is completely inside the other RectTransform</returns>
        public static bool IsFullyInside(this RectTransform rectTransform, RectTransform other, Canvas canvas = null)
        {
            if (!rectTransform || !other) return false;

            if(!canvas) canvas = rectTransform.GetComponentInParent<Canvas>();
            // Get the corners of both rects in world space
            var rectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(rectCorners);
        
            var otherCorners = new Vector3[4];
            other.GetWorldCorners(otherCorners);

            // Convert all corners to the canvas space
            for (var i = 0; i < 4; i++)
            {
                rectCorners[i] = canvas.transform.InverseTransformPoint(rectCorners[i]);
                otherCorners[i] = canvas.transform.InverseTransformPoint(otherCorners[i]);
            }

            // Get the bounds of the other rect in canvas space
            var otherBounds = new Bounds(otherCorners[0], Vector3.zero);
            for (var i = 1; i < 4; i++)
            {
                otherBounds.Encapsulate(otherCorners[i]);
            }

            // Check if all corners of the rect are inside the other bounds
            return rectCorners.All(corner => otherBounds.Contains(corner));
        }
    }
}