using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Extensions
{
    public static class EnemyAgentExtension
    {
        public static void AddOnDeadOnce(this EnemyAgent agent, Action onDead)
        {
            var called = false;
            agent.add_OnDeadCallback(new Action(()=>
            {
                if (called)
                    return;

                onDead?.Invoke();
                called = true;
            }));
        }
    }
}
