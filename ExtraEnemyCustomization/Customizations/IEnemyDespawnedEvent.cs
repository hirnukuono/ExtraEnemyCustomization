using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemyDespawnedEvent
    {
        void OnDespawned(EnemyAgent agent);
    }
}