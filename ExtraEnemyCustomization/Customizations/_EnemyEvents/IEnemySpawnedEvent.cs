using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemySpawnedEvent : IEnemyEvent
    {
        void OnSpawned(EnemyAgent agent);
    }
}