using EEC.Networking;
using Player;
using SNetwork;
using UnityEngine;

namespace EEC.CustomAbilities.Knockback
{
    internal sealed class KnockbackSync : SyncedPlayerEvent<KnockbackData>
    {
        public override string GUID => "KNB";
        public override bool SendToTargetOnly => true;
        public override bool AllowBots => false;

        protected override void Receive(KnockbackData packet, SNet_Player receivedPlayer)
        {
            if (!TryGetPlayerAgent(receivedPlayer, out var agent))
                return;
            DoKnockback(packet, agent);
        }

        private static void DoKnockback(KnockbackData data, PlayerAgent player)
        {
            var playerPos = player.Position;
            var powerVec = playerPos - data.inflictorPos;

            var distance = powerVec.magnitude;
            var direction = powerVec / distance;

            var velocity = direction * data.velocity;
            var velocityZ = Vector3.up * data.velocityZ;
            if (data.doMultDistance)
            {
                velocity *= distance;
            }

            if (data.doMultDistanceZ)
            {
                velocityZ *= distance;
            }

            player.Locomotion.AddExternalPushForce(velocity);

            if (data.velocityZ != 0.0f && player.Alive)
            {
                player.Locomotion.ChangeState(PlayerLocomotion.PLOC_State.Jump, true);
                player.Locomotion.VerticalVelocity = velocity + velocityZ;
            }
        }
    }
}