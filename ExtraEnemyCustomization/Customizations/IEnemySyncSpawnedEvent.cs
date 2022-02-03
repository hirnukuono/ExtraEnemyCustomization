using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemySyncSpawnedEvent : IEnemyEvent
    {
        void OnSyncSpawned(EnemyAgent agent);
    }
}