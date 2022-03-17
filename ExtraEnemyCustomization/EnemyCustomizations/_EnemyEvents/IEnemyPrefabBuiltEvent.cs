using Enemies;

namespace EECustom.EnemyCustomizations
{
    public interface IEnemyPrefabBuiltEvent : IEnemyEvent
    {
        void OnPrefabBuilt(EnemyAgent agent);
    }
}