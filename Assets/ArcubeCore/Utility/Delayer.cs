using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Arcube
{
    public class Delayer: MonoBehaviour
    {
        private static Delayer New => new GameObject().AddComponent<Delayer>();
        public static void Delay(float delay, Action callback)
        {
            var delayer = New;
            delayer.StartCoroutine(delayer.DelayCR(delay, callback));
        }

        [SerializeField] private float delay;
        public UnityEvent OnDelayEnded;
        public void Delay()
        {
            StartCoroutine(DelayCR(delay, () =>
            {
                OnDelayEnded?.Invoke();
            }));
        }

        private IEnumerator DelayCR(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
            Destroy(gameObject);
        }
    }
}