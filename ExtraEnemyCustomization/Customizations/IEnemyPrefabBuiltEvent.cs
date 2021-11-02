using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemyPrefabBuiltEvent : IEnemyEvent
    {
        void OnPrefabBuilt(EnemyAgent agent);
    }
}