using EECustom.Managers;
using GameData;
using System;
using System.Linq;

namespace EECustom.Customizations
{
    public sealed class TargetSetting
    {
        public TargetMode Mode { get; set; } = TargetMode.PersistentID;
        public uint[] PersistentIDs { get; set; } = new uint[1] { 0 };
        public string NameParam { get; set; } = string.Empty;
        public bool NameIgnoreCase { get; set; } = false;
        public string[] Categories { get; set; } = Array.Empty<string>();

        public bool IsMatch(uint enemyID)
        {
            var enemyBlock = GameDataBlockBase<EnemyDataBlock>.GetBlock(enemyID);
            return IsMatch(enemyBlock);
        }

        public bool IsMatch(EnemyDataBlock enemyBlock)
        {
            if (enemyBlock == null)
                return false;

            return Mode switch
            {
                TargetMode.PersistentID => PersistentIDs.Contains(enemyBlock.persistentID),
                TargetMode.NameEquals => enemyBlock.name?.InvariantEquals(NameParam, ignoreCase: NameIgnoreCase) ?? false,
                TargetMode.NameContains => enemyBlock.name?.InvariantContains(NameParam, ignoreCase: NameIgnoreCase) ?? false,
                TargetMode.Everything => true,
                TargetMode.CategoryAny => ConfigManager.Categories.Any(Categories, enemyBlock.persistentID),
                TargetMode.CategoryAll => ConfigManager.Categories.All(Categories, enemyBlock.persistentID),
                _ => false,
            };
        }

        public string ToDebugString()
        {
            return $"TargetDebug, Mode: {Mode}, persistentIDs: [{string.Join(", ", PersistentIDs)}], nameParam: {NameParam}, categories: [{string.Join(", ", Categories)}]";
        }
    }

    public enum TargetMode
    {
        PersistentID,
        NameEquals,
        NameContains,
        Everything,
        CategoryAny,
        CategoryAll
    }
}