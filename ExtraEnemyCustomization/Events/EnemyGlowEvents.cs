using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Events
{
    public class EnemyGlowEvents
    {
        private static Dictionary<ushort, Func<EnemyAgent, Color, Vector4, Color>> OnGlow = new Dictionary<ushort, Func<EnemyAgent, Color, Vector4, Color>>();

        public static Color FireEvent(EnemyAgent agent, Color color, Vector4 location)
        {
            if (OnGlow.TryGetValue(agent.GlobalID, out var func))
            {
                return func?.Invoke(agent, color, location) ?? color;
            }

            return color;
        }

        public static void RegisterOnGlow(EnemyAgent agent, Func<EnemyAgent, Color, Vector4, Color> onGlow)
        {
            var id = agent.GlobalID;
            if (OnGlow.ContainsKey(id))
            {
                OnGlow[id] += onGlow;
            }
            else
            {
                OnGlow.Add(id, onGlow);
            }

            agent.add_OnDeadCallback(new Action(() =>
            {
                OnGlow.Remove(id);
            }));
        }
    }
}
