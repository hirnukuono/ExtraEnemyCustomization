using EECustom.Customizations;
using EECustom.Customizations.Models;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class ModelCustomConfig : CustomizationConfig
    {
        public bool CacheAllMaterials { get; set; } = false;

        public ShadowCustom[] ShadowCustom { get; set; } = new ShadowCustom[0];
        public MaterialCustom[] MaterialCustom { get; set; } = new MaterialCustom[0];
        public GlowCustom[] GlowCustom { get; set; } = new GlowCustom[0];
        public LimbCustom[] LimbCustom { get; set; } = new LimbCustom[0];
        public ModelRefCustom[] ModelRefCustom { get; set; } = new ModelRefCustom[0];
        public MarkerCustom[] MarkerCustom { get; set; } = new MarkerCustom[0];
        public ScannerCustom[] ScannerCustom { get; set; } = new ScannerCustom[0];
        public SilhouetteCustom[] SilhouetteCustom { get; set; } = new SilhouetteCustom[0];

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
            return list.ToArray();
        }
    }
}