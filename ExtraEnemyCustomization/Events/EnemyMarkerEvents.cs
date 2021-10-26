using EECustom.Extensions;
using Enemies;
using System;

namespace EECustom.Events
{
    public static class EnemyMarkerEvents
    {
        public static Action<EnemyAgent, NavMarker> OnMarked;

        public static void RegisterOnMarked(EnemyAgent agent, Action<EnemyAgent, NavMarker> onMarked)
        {
            var id = agent.GlobalID;
            var onMarkedWrapper = new Action<EnemyAgent, NavMarker>((EnemyAgent eventAgent, NavMarker mark) =>
            {
                if (eventAgent.GlobalID == id)
                {
                    onMarked?.Invoke(eventAgent, mark);
                }
            });
            OnMarked += onMarkedWrapper;
            agent.AddOnDeadOnce(() =>
            {
                OnMarked -= onMarkedWrapper;
            });
        }
    }
}