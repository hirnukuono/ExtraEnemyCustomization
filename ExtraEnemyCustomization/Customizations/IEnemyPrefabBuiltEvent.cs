using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemyPrefabBuiltEvent
    {
        void OnPrefabBuilt(EnemyAgent agent);
    }
}