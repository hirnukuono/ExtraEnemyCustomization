namespace EECustom.Events
{
    public delegate void LevelEventHandler();

    public static class LevelEvents
    {
        public static event LevelEventHandler BuildStart;
        public static event LevelEventHandler BuildDone;
        public static event LevelEventHandler LevelCleanup;

        internal static void OnBuildStart()
        {
            BuildStart?.Invoke();
        }

        internal static void OnBuildDone()
        {
            BuildDone?.Invoke();
        }

        internal static void OnLevelCleanup()
        {
            LevelCleanup?.Invoke();
        }
    }
}