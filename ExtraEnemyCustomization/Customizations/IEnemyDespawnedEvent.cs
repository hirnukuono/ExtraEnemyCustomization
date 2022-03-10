using Enemies;
using System;

namespace EECustom.Customizations
{
    public interface IEnemyDespawnedEvent : IEnemyEvent
    {
        void OnDespawned(EnemyAgent agent);
    }
}