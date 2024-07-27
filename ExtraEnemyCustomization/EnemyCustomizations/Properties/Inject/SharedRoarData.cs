using EEC.EnemyCustomizations.Properties;

namespace ExtraEnemyCustomization.EnemyCustomizations.Properties.Inject
{
    internal static class SharedRoarData
    {
        public static Dictionary<uint, RoarData> Dict = new();

        public static DistantRoarCustom? Condense(List<KeyValuePair<uint, RoarData>>? filter)
        {
            if (filter == null || !filter.Any())
                return null;

            foreach (var entry in filter)
                if (Dict.TryGetValue(entry.Key, out var roarData))
                    roarData.IsInWave = false;

            return filter.OrderByDescending(entry => entry.Value.RoarSettings.RoarSize).FirstOrDefault().Value.RoarSettings;
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
