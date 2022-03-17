using Enemies;

namespace EECustom.EnemyCustomizations
{
    public interface IEnemyDespawnedEvent : IEnemyEvent
    {
        void OnDespawned(EnemyAgent agent);
    }
}