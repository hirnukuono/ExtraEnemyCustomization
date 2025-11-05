using EEC.Networking;

namespace EEC.CustomAbilities.Explosion
{
    internal sealed class ExplosionSync : SyncedEvent<ExplosionData>
    {
        public override string GUID => "EXP";

        protected override void Receive(ExplosionData packet)
        {
            Logger.Verbose($"Explosion Received: [{packet.position}] {packet.damage} {packet.enemyMulti} {packet.minRange} {packet.maxRange}");
            ExplosionManager.Internal_TriggerHostOnlyExplosion(packet.position, packet.lightColor, packet.damage, packet.enemyMulti, packet.minRange, packet.maxRange, packet.enemyMinRange, packet.enemyMaxRange);
        }
    }

    internal sealed class ExplosionAgentSync : SyncedEvent<ExplosionAgentData>
    {
        public override string GUID => "EXPA";

        protected override void Receive(ExplosionAgentData packet)
        {
            if (!packet.agent.TryGet(out var agent)) return;

            var position = agent.EyePosition;
            if (packet.useRagdoll)
                agent.GetRagdollPosition(ref position);
            ExplosionManager.Internal_TriggerExplosion(position, packet.lightColor, packet.damage, packet.enemyMulti, packet.minRange, packet.maxRange, packet.enemyMinRange, packet.enemyMaxRange);
        }
    }
}