using EECustom.Utils;
using UnityEngine;

namespace EECustom.Networking.Events
{
    public class ExplosionEvent : SyncedEvent<ExplosionPacket>
    {
        public override void Receive(ExplosionPacket packet)
        {
            Logger.Verbose($"Explosion Received: [{packet.position}] {packet.damage} {packet.enemyMulti} {packet.minRange} {packet.maxRange}");
            ExplosionUtil.Internal_TriggerExplosion(packet.position, packet.damage, packet.enemyMulti, packet.minRange, packet.maxRange);
        }
    }

    public struct ExplosionPacket
    {
        public Vector3 position;
        public float damage;
        public float enemyMulti;
        public float minRange;
        public float maxRange;
    }
}
