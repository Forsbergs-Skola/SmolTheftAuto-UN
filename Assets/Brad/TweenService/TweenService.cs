using UnityEngine;
namespace Tweens
{
    public interface ITween
    {
        public void StartTween();
        public void PauseTween();
        public void SetRepeat(bool value);
        public void ResetTween();
    }

    public enum EnumTweenEase { LINEAR, QUAD, CUBIC, QUART, SINE };
    public enum EnumTweenDirection { IN, OUT, IN_OUT };


    public static class TweenService
    {

        public const float MINIMUM_DURATION = 0.01f;
        public static Tween GetFloatTween
            (
                GameObject owner,
                float startValue,
                float endValue,
                float duration,
                EnumTweenEase ease = EnumTweenEase.LINEAR,
                EnumTweenDirection direction = EnumTweenDirection.IN_OUT,
                bool autodestruct = true
            )
        {
            var tweenObj = new GameObject("TweenFloat");
            var tween = tweenObj.AddComponent<Tween>();
            Vector3 startV3 = new Vector3(startValue, 0.0f, 0.0f);
            Vector3 endv3 = new Vector3(endValue, 0.0f, 0.0f);
            tween.Initialize(owner, startV3, endv3, duration, ease, direction, autodestruct);
            tween.transform.parent = owner.transform;
            return tween;
        }
        public static Tween GetVector2Tween
            (
                GameObject owner,
                Vector2 startValue,
                Vector2 endValue,
                float duration,
                EnumTweenEase ease = EnumTweenEase.LINEAR,
                EnumTweenDirection direction = EnumTweenDirection.IN_OUT,
                bool autodestruct = true
            )
        {
            var tweenObj = new GameObject("TweenFloat");
            var tween = tweenObj.AddComponent<Tween>();
            Vector3 startV3 = new Vector3(startValue.x, startValue.y, 0.0f);
            Vector3 endv3 = new Vector3(endValue.x, endValue.y, 0.0f);
            tween.Initialize(owner, startV3, endv3, duration, ease, direction, autodestruct);
            tween.transform.parent = owner.transform;
            return tween;
        }

        public static Tween GetVector3Tween
            (
                GameObject owner,
                Vector3 startValue,
                Vector3 endValue,
                float duration,
                EnumTweenEase ease = EnumTweenEase.LINEAR,
                EnumTweenDirection direction = EnumTweenDirection.IN_OUT,
                bool autodestruct = true
            )
        {
            var tweenObj = new GameObject("TweenFloat");
            var tween = tweenObj.AddComponent<Tween>();
            tween.Initialize(owner, startValue, endValue, duration, ease, direction, autodestruct);
            tween.transform.parent = owner.transform;
            return tween;
        }
        public static System.Func<float, float> GetEaseFunction(EnumTweenEase ease, EnumTweenDirection direction)
        {
            switch (ease)
            {
                case EnumTweenEase.QUAD:
                    return direction switch
                    {
                        EnumTweenDirection.IN => t => t * t,
                        EnumTweenDirection.OUT => t => 1 - ((1 - t) * (1 - t)),
                        EnumTweenDirection.IN_OUT => t => t < 0.5f ? 2 * t * t : 1 - (Mathf.Pow(-2 * t + 2, 2) / 2),
                        _ => t => t, // default
                    };
                case EnumTweenEase.SINE:
                    return direction switch
                    {
                        EnumTweenDirection.IN => t => 1 - Mathf.Cos((t * Mathf.PI) / 2),
                        EnumTweenDirection.OUT => t => Mathf.Sin((t * Mathf.PI) / 2),
                        EnumTweenDirection.IN_OUT => t => -(Mathf.Cos(Mathf.PI * t) - 1) / 2,
                        _ => t => t // default
                    };
                case EnumTweenEase.CUBIC:
                    return direction switch
                    {
                        EnumTweenDirection.IN => t => t * t * t,
                        EnumTweenDirection.OUT => t => 1 - ((1 - t) * (1 - t) * (1 - t)),
                        EnumTweenDirection.IN_OUT => t => t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2,
                        _ => t => t, // default
                    };
                case EnumTweenEase.QUART:
                    return direction switch
                    {
                        EnumTweenDirection.IN => t => t * t * t * t,
                        EnumTweenDirection.OUT => t => 1 - ((1 - t) * (1 - t) * (1 - t) * (1 - t)),
                        EnumTweenDirection.IN_OUT => t => t < 0.5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2,
                        _ => t => t, // default
                    };
                default:
                    return t => t; // "return a function that intakes a number and returns that number"

            }
        }
    }
}