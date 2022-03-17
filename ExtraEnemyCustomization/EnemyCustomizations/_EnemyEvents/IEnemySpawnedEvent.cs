using Enemies;

namespace EEC.EnemyCustomizations
{
    public interface IEnemySpawnedEvent : IEnemyEvent
    {
        void OnSpawned(EnemyAgent agent);
    }
}