using Player;

namespace EEC.Events
{
    public delegate void PlayerAliveHandler(PlayerAgent playerAgent);

    public static class LocalPlayerAliveEvents
    {
        public static event PlayerAliveHandler Down;

        public static event PlayerAliveHandler Revive;

        internal static void OnDown(PlayerAgent playerAgent)
        {
            Down?.Invoke(playerAgent);
        }

        internal static void OnRevive(PlayerAgent playerAgent)
        {
            Revive?.Invoke(playerAgent);
        }
    }
}