using Enemies;

namespace EEC.EnemyCustomizations
{
    public interface IEnemyDeadEvent : IEnemyEvent
    {
        void OnDead(EnemyAgent agent);
    }
}