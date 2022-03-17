using Enemies;

namespace EECustom.EnemyCustomizations
{
    public interface IEnemySpawnedEvent : IEnemyEvent
    {
        void OnSpawned(EnemyAgent agent);
    }
}