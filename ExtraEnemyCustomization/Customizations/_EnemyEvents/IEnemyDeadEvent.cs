using Enemies;

namespace EECustom.Customizations
{
    public interface IEnemyDeadEvent : IEnemyEvent
    {
        void OnDead(EnemyAgent agent);
    }
}