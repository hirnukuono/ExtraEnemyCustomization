using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace EEC.Utils.Json.Elements
{
    public sealed class CurveWrapper : Collection<CurveKeyFrame>
    {
        public const int KEYFRAME_FRAMECOUNT = 20;
        public const float KEYFRAME_PROGRESS_INV = 1.0f / KEYFRAME_FRAMECOUNT;

        public static readonly CurveWrapper Empty = new();

        private static readonly List<Keyframe> _keys = new();

        public bool HasSetting => (Count >= 2);

        public bool TryBuildCurve(out AnimationCurve curve)
        {
            if (Count < 2)
            {
                curve = null;
                return false;
            }

            var sorted = this.OrderBy(x => x.Time).ToArray();
            _keys.Clear();

            for (int i = 0; i < Count - 1; i++)
            {
                var item = sorted[i];
                var nextItem = sorted[i + 1];

                if (item.Time > 1.0f || item.Time < 0.0f)
                {
                    Logger.Error($"CurveWrapper Time '{item.Time}' was invalid!, must be range of 0.0 ~ 1.0");
                    curve = null;
                    return false;
                }

                var deltaTime = nextItem.Time - item.Time;
                var deltaValue = nextItem.Value - item.Value;
                var slope = deltaValue / deltaTime;
                for (int j = 0; j < KEYFRAME_FRAMECOUNT; j++)
                {
                    var progress = KEYFRAME_PROGRESS_INV * j;
                    var time = item.Time + (deltaTime * progress);
                    var value = item.Value + (deltaValue * item.OutEaseType.Evaluate(progress));
                    _keys.Add(new Keyframe()
                    {
                        time = time,
                        value = value,
                        inTangent = slope,
                        outTangent = slope
                    });
                }
            }

            curve = new AnimationCurve(_keys.ToArray());
            return true;
        }
    }

    public struct CurveKeyFrame
    {
        public float Time { get; set; }
        public float Value { get; set; }
        public eEasingType OutEaseType { get; set; }
    }
}