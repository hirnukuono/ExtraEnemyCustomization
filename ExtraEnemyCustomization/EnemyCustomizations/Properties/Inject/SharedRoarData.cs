using EEC.EnemyCustomizations.Properties;
using System.Diagnostics.CodeAnalysis;

namespace EEC.EnemyCustomizations.Properties.Inject
{
    internal static class SharedRoarData
    {
        public static Dictionary<uint, RoarData> Dict = new();

        public static bool TryCondense(byte enemyType, [NotNullWhen(true)] out DistantRoarCustom? largest)
        {
            var filtered = Dict.Where(entry => entry.Value.EnemyType == enemyType && entry.Value.IsInWave).ToList();

            if (!filtered.Any())
            {
                largest = null;
                return false;
            }

            foreach (var entry in filtered)
            {
                entry.Value.IsInWave = false;
            }

            largest = filtered.OrderByDescending(entry => entry.Value.RoarSettings.RoarSize).FirstOrDefault().Value.RoarSettings;
            return largest != null;
        }

        internal class RoarData
        {
            public DistantRoarCustom RoarSettings { get; set; } = new();
            public uint SwitchID { get; set; }
            public byte EnemyType { get; set; }
            public bool IsInWave { get; set; } = false;
        }
    }
}
