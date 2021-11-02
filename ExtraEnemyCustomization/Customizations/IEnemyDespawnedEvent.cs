using Enemies;
using System;

namespace EECustom.Customizations
{
    [Obsolete("Using Despawned Event will not work since ID will be unregiered on dead!", false)]
    public interface IEnemyDespawnedEvent : IEnemyEvent
    {
        void OnDespawned(EnemyAgent agent);
    }
}