using Player;
using UnityEngine;

namespace EEC.CustomAbilities.Knockback
{
    [CallConstructorOnLoad]
    public static class KnockbackManager
    {
        internal static KnockbackSync Sync { get; private set; } = new();

        static KnockbackManager()
        {
            Sync.Setup();
        }

        public static void DoKnockback(PlayerAgent agent, KnockbackData data)
        {
            Sync.SendToPlayer(data, agent);
        }
    }

    public struct KnockbackData
    {
        public Vector3 inflictorPos;
        public float velocity;
        public float velocityZ;
        public bool doMultDistance;
        public bool doMultDistanceZ;
    }
}