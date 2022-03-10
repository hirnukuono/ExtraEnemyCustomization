using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations
{
    public interface IEnemyDeadEvent : IEnemyEvent
    {
        void OnDead(EnemyAgent agent);
    }
}
