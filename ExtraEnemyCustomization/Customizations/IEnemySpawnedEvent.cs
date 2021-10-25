using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemySpawnedEvent
    {
        void OnSpawned(EnemyAgent agent);
    }
}