using System.Text.Json.Serialization;

namespace EECustom.Configs
{
    public sealed class GlobalConfig : Config
    {
        #region FlyerStuck

        [JsonPropertyName("Flyer.StuckCheck.Enabled")]
        public bool UsingFlyerStuckCheck { get; set; } = false;

        [JsonPropertyName("Flyer.StuckCheck.RetryCount")]
        public int FlyerStuck_Retry { get; set; } = 5;

        [JsonPropertyName("Flyer.StuckCheck.RetryInterval")]
        public float FlyerStuck_Interval { get; set; } = 1.5f;

        #endregion FlyerStuck

        #region Materials

        [JsonPropertyName("Material.CacheAll")]
        public bool CacheAllMaterials { get; set; } = false;

        #endregion Materials

        #region Bleeding

        [JsonPropertyName("Bleeding.UseMediToStop")]
        public bool CanMediStopBleeding { get; set; } = false;

        #endregion Bleeding

        public override string FileName => "Global";
    }
}