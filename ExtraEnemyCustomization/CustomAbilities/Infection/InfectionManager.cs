using EEC.EnemyCustomizations.Shared;
using Player;

namespace EEC.CustomAbilities.Infection
{
    [CallConstructorOnLoad]
    public static class InfectionManager
    {
        internal static InfectionSync Sync { get; private set; } = new();

        static InfectionManager()
        {
            Sync.Setup();
        }

        public static void DoInfection(PlayerAgent agent, IInfectionSetting setting) => DoInfection(agent, setting.ToPacket());

        public static void DoInfection(PlayerAgent agent, InfectionData data)
        {
            if (data.infection == 0) return;

            Sync.SendToPlayer(data, agent);
        }
    }

    public struct InfectionData
    {
        public float infection;
        public uint soundEventID;
        public bool useEffect;
        public float screenLiquidRange;
    }
}