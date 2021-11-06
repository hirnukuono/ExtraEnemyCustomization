using EECustom.Networking.Events;

namespace EECustom.Networking
{
    public static class NetworkManager
    {
        public static ExplosionEvent Explosion { get; private set; } = new();

        internal static void Initialize()
        {
            Explosion.Setup();
        }
    }
}
