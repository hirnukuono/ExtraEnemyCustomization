using Enemies;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public interface IAbility
    {
        string Name { get; set; }

        ushort SyncID { get; }

        void Setup(ushort syncID);

        void Unload();

        void TriggerSync(EnemyAgent agent) => TriggerSync(agent.GlobalID);

        void TriggerSync(ushort enemyID);

        void Trigger(EnemyAgent agent) => Trigger(agent.GlobalID);

        void Trigger(ushort enemyID);

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