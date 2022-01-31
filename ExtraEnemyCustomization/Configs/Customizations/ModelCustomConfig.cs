using EECustom.Customizations;
using EECustom.Customizations.Models;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class ModelCustomConfig : CustomizationConfig
    {
        public ShadowCustom[] ShadowCustom { get; set; } = Array.Empty<ShadowCustom>();
        public MaterialCustom[] MaterialCustom { get; set; } = Array.Empty<MaterialCustom>();
        public GlowCustom[] GlowCustom { get; set; } = Array.Empty<GlowCustom>();
        public LimbCustom[] LimbCustom { get; set; } = Array.Empty<LimbCustom>();
        public ModelRefCustom[] ModelRefCustom { get; set; } = Array.Empty<ModelRefCustom>();
        public MarkerCustom[] MarkerCustom { get; set; } = Array.Empty<MarkerCustom>();
        public ScannerCustom[] ScannerCustom { get; set; } = Array.Empty<ScannerCustom>();
        public SilhouetteCustom[] SilhouetteCustom { get; set; } = Array.Empty<SilhouetteCustom>();
        public BoneCustom[] BoneCustom { get; set; } = Array.Empty<BoneCustom>();

        public override string FileName => "Model";
        public override CustomizationConfigType Type => CustomizationConfigType.Model;

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(ShadowCustom);
            list.AddRange(MaterialCustom);
            list.AddRange(GlowCustom);
            list.AddRange(LimbCustom);
            list.AddRange(ModelRefCustom);
            list.AddRange(MarkerCustom);
            list.AddRange(ScannerCustom);
            list.AddRange(SilhouetteCustom);
            list.AddRange(BoneCustom);
            return list.ToArray();
        }
    }
}