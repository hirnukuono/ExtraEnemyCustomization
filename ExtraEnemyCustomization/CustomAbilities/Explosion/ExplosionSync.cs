using EEC.Networking;

namespace EEC.CustomAbilities.Explosion
{
    internal sealed class ExplosionSync : SyncedEvent<ExplosionData>
    {
        public override string GUID => "EXP";

        protected override void Receive(ExplosionData packet)
        {
            Logger.Verbose($"Explosion Received: [{packet.position}] {packet.damage} {packet.enemyMulti} {packet.minRange} {packet.maxRange}");
            ExplosionManager.Internal_TriggerExplosion(packet.position, packet.lightColor, packet.damage, packet.enemyMulti, packet.minRange, packet.maxRange, packet.enemyMinRange, packet.enemyMaxRange);
        }
    }
}