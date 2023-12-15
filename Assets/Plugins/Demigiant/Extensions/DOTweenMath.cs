using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace DG.Tweening
{
    public readonly struct DOTweenMath
    {
        public static TweenerCore<float, float, FloatOptions> Lerp(float startValue, float endValue, float duration, Action<float> onUpdate)
        {
            var t = DOTween.To(() => startValue, x => startValue = x, endValue, duration).OnUpdate(() => onUpdate(startValue));
            return t;
        }
    }
}