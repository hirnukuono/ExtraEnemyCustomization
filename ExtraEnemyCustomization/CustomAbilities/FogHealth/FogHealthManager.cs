using Player;

namespace EEC.CustomAbilities.FogHealth
{
    [CallConstructorOnLoad]
    public static class FogHealthManager
    {
        internal static FogHealthSync Sync { get; private set; } = new();

        static FogHealthManager()
        {
            Sync.Setup();
        }

        public static void DoHealthChange(PlayerAgent agent, float amount)
        {
            if (amount > 0) // Sends to host -> host sets & sends health -> clients receive new health
                agent.Damage.AddHealth(amount, null);
            else // Sends to all players -> everyone sets the player as having taken damage
                Sync.SendToPlayer(new FogHealthData() { damage = -amount }, agent);
        }
    }

    public struct FogHealthData
    {
        public float damage;
    }
}