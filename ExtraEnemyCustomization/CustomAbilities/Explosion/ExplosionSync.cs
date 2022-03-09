using EECustom.Networking;
using EECustom.Utils;
using UnityEngine;

namespace EECustom.CustomAbilities.Explosion
{
    internal sealed class ExplosionSync : SyncedEvent<ExplosionData>
    {
        public override string GUID => "EXP";

        public override void Receive(ExplosionData packet)
        {
            Logger.Verbose($"Explosion Received: [{packet.position}] {packet.damage} {packet.enemyMulti} {packet.minRange} {packet.maxRange}");
            ExplosionManager.Internal_TriggerExplosion(packet.position, packet.damage, packet.enemyMulti, packet.minRange, packet.maxRange);
        }
    }
}