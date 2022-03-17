using SNetwork;
using System;

namespace EECustom.Networking
{
    public delegate void SNetRecallEvent(eBufferType bufferType);

    public delegate void SNetPlayerEvent(SNet_Player player);

    public delegate void SNetPlayerEventWithReason(SNet_Player player, SNet_PlayerEventReason reason);

    public static class SNetEvents
    {
        public static event SNetPlayerEvent AgentSpawned;
        public static event SNetRecallEvent PrepareRecall;
        public static event SNetRecallEvent RecallComplete;

        internal static void OnAgentSpawned(SNet_Player player)
        {
            AgentSpawned?.Invoke(player);
        }

        internal static void OnPrepareRecall(eBufferType bufferType)
        {
            PrepareRecall?.Invoke(bufferType);
        }

        internal static void OnRecallComplete(eBufferType bufferType)
        {
            RecallComplete?.Invoke(bufferType);
        }
    }
}