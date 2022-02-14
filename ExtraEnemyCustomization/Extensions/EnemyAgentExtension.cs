using EECustom.Utils;
using Enemies;
using System;

namespace EECustom
{
    public static class EnemyAgentExtension
    {
        public static void AddOnDeadOnce(this EnemyAgent agent, Action onDead)
        {
            var called = false;
            agent.add_OnDeadCallback(new Action(() =>
            {
                if (called)
                    return;

                onDead?.Invoke();
                called = true;
            }));
        }

        public static T RegisterProperty<T>(this EnemyAgent agent) where T : class, new()
        {
            return EnemyProperty<T>.RegisterOrGet(agent);
        }

        public static bool TryGetProperty<T>(this EnemyAgent agent, out T property) where T : class, new()
        {
            return EnemyProperty<T>.TryGet(agent, out property);
        }
    }
}