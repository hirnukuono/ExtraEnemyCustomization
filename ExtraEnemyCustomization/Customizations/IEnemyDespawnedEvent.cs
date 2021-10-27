using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemyDespawnedEvent : IEnemyEvent
    {
        void OnDespawned(EnemyAgent agent);
    }
}