using Agents;
using EEC.CustomAbilities.Knockback;
using Player;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public interface IKnockbackSetting
    {
        public float Velocity { get; set; }
        public float VelocityZ { get; set; }
        public bool DoMultDistance { get; set; }
        public bool DoMultDistanceZ { get; set; }
    }

    public sealed class KnockbackSetting : IKnockbackSetting
    {
        public bool Enabled { get; set; } = false;
        public float Velocity { get; set; } = 0.0f;
        public float VelocityZ { get; set; } = 0.0f;
        public bool DoMultDistance { get; set; } = false;
        public bool DoMultDistanceZ { get; set; } = false;

        public KnockbackData ToPacket(Vector3 inflictorPos)
        {
            return new KnockbackData()
            {
                inflictorPos = inflictorPos,
                velocity = Velocity,
                velocityZ = VelocityZ,
                doMultDistance = DoMultDistance,
                doMultDistanceZ = DoMultDistanceZ,
            };
        }

        public void DoKnockback(Agent inflictor, PlayerAgent player) => DoKnockback(inflictor.Position, player);

        public void DoKnockback(Vector3 inflictorPos, PlayerAgent player)
        {
            KnockbackManager.DoKnockback(player, ToPacket(inflictorPos));
        }

        public void DoKnockbackIgnoreDistance(Agent inflictor, PlayerAgent player) => DoKnockbackIgnoreDistance(inflictor.Position, player);

        public void DoKnockbackIgnoreDistance(Vector3 inflictorPos, PlayerAgent player)
        {
            var packet = ToPacket(inflictorPos);
            packet.doMultDistance = false;
            packet.doMultDistanceZ = false;
            KnockbackManager.DoKnockback(player, packet);
        }
    }
}