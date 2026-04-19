using EEC.CustomAbilities.Infection;
using EEC.Utils;
using EEC.Utils.Json.Elements;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IInfectionSetting
    {
        public ValueBase Infection { get; set; }
        public uint SoundEventID { get; set; }
        public bool UseEffect { get; set; }
        public float ScreenLiquidRange { get; set; }

        public InfectionData ToPacket()
        {
            return new InfectionData()
            {
                infection = Infection.GetAbsValue(PlayerData.MaxInfection),
                soundEventID = SoundEventID,
                useEffect = UseEffect,
                screenLiquidRange = ScreenLiquidRange,
            };
        }
    }

    public sealed class InfectionSetting : IInfectionSetting
    {
        public bool Enabled { get; set; } = false;
        public ValueBase Infection { get; set; } = ValueBase.Zero;
        public uint SoundEventID { get; set; } = 0u;
        public bool UseEffect { get; set; } = false;
        public float ScreenLiquidRange { get; set; } = 0.0f;
    }
}