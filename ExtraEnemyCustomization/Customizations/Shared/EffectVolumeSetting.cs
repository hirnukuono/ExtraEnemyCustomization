using UnityEngine;

namespace EECustom.Customizations.Shared
{
    public sealed class EffectVolumeSetting
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