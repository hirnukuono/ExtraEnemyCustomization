using System;

namespace EECustom
{
    using EaseFunc = Func<float, float, float, float, float>;

    public static class EasingExtension
    {
        public static float Evaluate(this eEasingType easeType, float progress, bool backward = false)
        {
            return Easing.GetEasingValue(easeType, progress, backward);
        }

        public static EaseFunc GetEaseFunction(this eEasingType easeType)
        {
            return easeType switch
            {
                eEasingType.EaseInQuad => new EaseFunc(Easing.EaseInQuad),
                eEasingType.EaseOutQuad => new EaseFunc(Easing.EaseOutQuad),
                eEasingType.EaseInOutQuad => new EaseFunc(Easing.EaseInOutQuad),
                eEasingType.EaseInCubic => new EaseFunc(Easing.EaseInCubic),
                eEasingType.EaseOutCubic => new EaseFunc(Easing.EaseOutCubic),
                eEasingType.EaseInOutCubic => new EaseFunc(Easing.EaseInOutCubic),
                eEasingType.EaseInQuart => new EaseFunc(Easing.EaseInQuart),
                eEasingType.EaseOutQuart => new EaseFunc(Easing.EaseOutQuart),
                eEasingType.EaseInOutQuart => new EaseFunc(Easing.EaseInOutQuart),
                eEasingType.EaseInQuint => new EaseFunc(Easing.EaseInQuint),
                eEasingType.EaseOutQuint => new EaseFunc(Easing.EaseOutQuint),
                eEasingType.EaseInOutQuint => new EaseFunc(Easing.EaseInOutQuint),
                eEasingType.EaseInSine => new EaseFunc(Easing.EaseInSine),
                eEasingType.EaseOutSine => new EaseFunc(Easing.EaseOutSine),
                eEasingType.EaseInOutSine => new EaseFunc(Easing.EaseInOutSine),
                eEasingType.EaseInExpo => new EaseFunc(Easing.EaseInExpo),
                eEasingType.EaseOutExpo => new EaseFunc(Easing.EaseOutExpo),
                eEasingType.EaseInOutExpo => new EaseFunc(Easing.EaseInOutExpo),
                eEasingType.EaseInCirc => new EaseFunc(Easing.EaseInCirc),
                eEasingType.EaseOutCirc => new EaseFunc(Easing.EaseOutCirc),
                eEasingType.EaseInOutCirc => new EaseFunc(Easing.EaseInOutCirc),
                _ => new EaseFunc(Easing.LinearTween),
            };
        }
    }
}