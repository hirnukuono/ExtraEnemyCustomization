using Enemies;

namespace EEC.EnemyCustomizations
{
    public interface IEnemyDespawnedEvent : IEnemyEvent
    {
        void OnDespawned(EnemyAgent agent);
    }
}