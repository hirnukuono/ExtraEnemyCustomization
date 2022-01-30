using SNetwork;

namespace EECustom.Networking
{
    public delegate void SNetPlayerEvent(SNet_Player player);

    public delegate void SNetPlayerEventWithReason(SNet_Player player, SNet_PlayerEventReason reason);

    public static class SNetEvents
    {
        public static event SNetPlayerEvent AgentSpawned;

        internal static void OnAgentSpawned(SNet_Player player)
        {
            AgentSpawned?.Invoke(player);
        }
    }
}