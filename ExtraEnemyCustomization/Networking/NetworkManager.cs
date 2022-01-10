using EECustom.Networking.Events;

namespace EECustom.Networking
{
    public static class NetworkManager
    {
        public static ExplosionEvent Explosion { get; private set; } = new();
        public static BleedingEvent Bleeding { get; private set; } = new();
        public static EnemyAnimEvent EnemyAnim { get; private set; } = new();

        internal static void Initialize()
        {
            Explosion.Setup();
            Bleeding.Setup();
            EnemyAnim.Setup();
        }
    }
}
