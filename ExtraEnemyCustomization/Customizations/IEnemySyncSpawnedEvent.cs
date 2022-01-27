using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations
{
    public interface IEnemySyncSpawnedEvent : IEnemyEvent
    {
        void OnSyncSpawned(EnemyAgent agent);
    }
}
