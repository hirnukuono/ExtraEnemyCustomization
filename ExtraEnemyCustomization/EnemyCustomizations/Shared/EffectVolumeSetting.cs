using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IEffectVolumeSetting
    {
        public bool Enabled { get; set; }
        public eEffectVolumeContents Contents { get; set; }
        public eEffectVolumeModification Modification { get; set; }
        public float Scale { get; set; }
    }

    public sealed class EffectVolumeSetting : IEffectVolumeSetting
    {
        public bool Enabled { get; set; } = false;
        public eEffectVolumeContents Contents { get; set; } = eEffectVolumeContents.Infection;
        public eEffectVolumeModification Modification { get; set; } = eEffectVolumeModification.Inflict;
        public float Scale { get; set; } = 1f;

        public EV_Sphere CreateSphere(Vector3 position, float minRadius, float maxRadius)
        {
            return new EV_Sphere()
            {
                position = position,
                minRadius = minRadius,
                maxRadius = maxRadius,
                modificationScale = Scale,
                invert = true,
                contents = Contents,
                modification = Modification
            };
        }
    }
}