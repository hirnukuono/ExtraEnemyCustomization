using Enemies;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public interface IAbility
    {
        string Name { get; set; }

        ushort SyncID { get; }

        void Setup(ushort syncID);

        void Unload();

        void TriggerSync(EnemyAgent agent, bool clientPos = false) => TriggerSync(agent.GlobalID, clientPos);

        void TriggerSync(ushort enemyID, bool clientPos = false);

        void Trigger(EnemyAgent agent, bool clientPos) => Trigger(agent.GlobalID, clientPos);

        void Trigger(ushort enemyID, bool clientPos);

        void TriggerAllSync();

        void TriggerAll();

        void ExitSync(EnemyAgent agent) => ExitSync(agent.GlobalID);

        void ExitSync(ushort enemyID);

        void Exit(EnemyAgent agent) => Exit(agent.GlobalID);

        void Exit(ushort enemyID);

        void ExitAllSync();

        void ExitAll();

        AbilityBehaviour RegisterBehaviour(EnemyAgent agent);

        bool TryGetBehaviour(EnemyAgent agent, out AbilityBehaviour behaviour) => TryGetBehaviour(agent.GlobalID, out behaviour);

        bool TryGetBehaviour(ushort enemyID, out AbilityBehaviour behaviour);

        AbilityBehaviour[] Behaviours { get; }
    }
}