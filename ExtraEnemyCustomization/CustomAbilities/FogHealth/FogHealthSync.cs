using EEC.Networking;
using SNetwork;

namespace EEC.CustomAbilities.FogHealth
{
    internal sealed class FogHealthSync : SyncedPlayerEvent<FogHealthData>
    {
        public override string GUID => "FGH";
        public override bool SendToTargetOnly => false;
        public override bool AllowBots => false;

        // Custom value for the amount the screen is flashed by damage.
        // Vanilla GTFO uses damage / 2f, but non-relativity is cringe.
        private const float FLASH_CONVERSION = 6f;

        protected override void Receive(FogHealthData packet, SNet_Player receivedPlayer)
        {
            if (TryGetPlayerAgent(receivedPlayer, out var agent))
            {
                if (Logger.VerboseLogAllowed)
                    Logger.Verbose($"FogHealth [{agent.PlayerSlotIndex} {packet.damage}]");

                var damage = packet.damage;
                var damBase = agent.Damage;
                if (receivedPlayer.IsLocal)
                    agent.FPSCamera.AddHitReact(damage / damBase.HealthMax * FLASH_CONVERSION, UnityEngine.Vector3.up, 0f);
                damBase.OnIncomingDamage(damage, damage);
            }
        }
    }
}