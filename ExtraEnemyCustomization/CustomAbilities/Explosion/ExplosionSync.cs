using EEC.Networking;

namespace EEC.CustomAbilities.Explosion
{
    internal sealed class ExplosionSync : SyncedEvent<ExplosionPosData>
    {
        public override string GUID => "EXP";

        protected override void Receive(ExplosionPosData packet)
        {
            var data = packet.data;
            Logger.Verbose($"Explosion Received: [{packet.position}] {data.damage} {data.enemyMulti} {data.minRange} {data.maxRange}");
            ExplosionManager.Internal_TriggerHostOnlyExplosion(packet.position, data);
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
            var data = packet.data;
            data.knockback.packet.inflictorPos = position;
            ExplosionManager.Internal_TriggerExplosion(position, data);
        }
    }
}