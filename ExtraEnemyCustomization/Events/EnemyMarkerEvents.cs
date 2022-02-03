using Enemies;

namespace EECustom.Events
{
    public delegate void EnemyMarkerHandler(EnemyAgent agent, NavMarker marker);

    public static class EnemyMarkerEvents
    {
        public static event EnemyMarkerHandler Marked;

        internal static void OnMarked(EnemyAgent agent, NavMarker marker)
        {
            Marked?.Invoke(agent, marker);
        }
    }
}