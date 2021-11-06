namespace EECustom.Events
{
    public delegate void EnemyLimbHandler(Dam_EnemyDamageLimb limb);

    public static class EnemyLimbEvents
    {
        public static event EnemyLimbHandler Destroyed;

        internal static void OnDestroyed(Dam_EnemyDamageLimb limb)
        {
            Destroyed?.Invoke(limb);
        }
    }
}
