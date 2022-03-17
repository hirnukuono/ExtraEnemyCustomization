using Enemies;

namespace EEC.EnemyCustomizations
{
    public interface IEnemyPrefabBuiltEvent : IEnemyEvent
    {
        void OnPrefabBuilt(EnemyAgent agent);
    }
}