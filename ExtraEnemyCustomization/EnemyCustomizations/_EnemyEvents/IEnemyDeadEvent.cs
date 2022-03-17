using Enemies;

namespace EECustom.EnemyCustomizations
{
    public interface IEnemyDeadEvent : IEnemyEvent
    {
        void OnDead(EnemyAgent agent);
    }
}