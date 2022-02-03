using BepInEx.Logging;

namespace EECustom
{
    public static class Logger
    {
        public static ManualLogSource LogInstance { get; private set; }
        public static bool UsingDevMessage { get; private set; } = false;
        public static bool UsingVerbose { get; private set; } = false;

        internal static void Initialize()
        {
            LogInstance = new ManualLogSource("EEC");
            BepInEx.Logging.Logger.Sources.Add(LogInstance);

            UsingDevMessage = Configuration.UseDebugLog.Value;
            UsingVerbose = Configuration.UseVerboseLog.Value;
        }

        public static void Log(string format, params object[] args) => Log(string.Format(format, args));

        public static void Log(string str)
        {
            LogInstance?.Log(LogLevel.Message, str);
        }

        public static void Warning(string format, params object[] args) => Warning(string.Format(format, args));

        public static void Warning(string str)
        {
            LogInstance?.Log(LogLevel.Warning, str);
        }

        public static void Error(string format, params object[] args) => Error(string.Format(format, args));

        public static void Error(string str)
        {
            LogInstance?.Log(LogLevel.Error, str);
        }

        public static void Debug(string format, params object[] args) => Debug(string.Format(format, args));

        public static void Debug(string str)
        {
            if (UsingDevMessage)
                LogInstance?.LogDebug(str);
        }

        public static void Verbose(string format, params object[] args) => Verbose(string.Format(format, args));

        public static void Verbose(string str)
        {
            if (UsingDevMessage && UsingVerbose)
                LogInstance?.LogDebug($"{str} (Verbose)");
        }
    }
}