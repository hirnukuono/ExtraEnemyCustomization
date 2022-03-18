using Enemies;
using GameData;

namespace EEC.EnemyCustomizations
{
    public interface IEnemyPrefabBuiltEvent : IEnemyEvent
    {
        void OnPrefabBuilt(EnemyAgent agent, EnemyDataBlock enemyData);
    }
}