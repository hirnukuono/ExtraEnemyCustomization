using AK;
using ExtraEnemyCustomization.Extensions;
using HarmonyLib;
using LevelGeneration;
using System.Diagnostics.CodeAnalysis;

namespace EEC.EnemyCustomizations.Properties.Inject
{
    [HarmonyPatch(typeof(LG_LevelInteractionManager), nameof(LG_LevelInteractionManager.DoSetWaveRoarSoundInformation))]
    internal static class Inject_DoSetWaveRoar
    {
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static bool Pre_DoSetWaveRoarSound(LG_LevelInteractionManager.pWaveRoarSettings settings)
        {
            if (!TryCondense(settings.enemyType, out var largest))
            {
                return true;
            }

            CellSoundPlayer waveRoar = new();
            int size = largest.RoarSize == 0 ? settings.roarSize : (int)largest.RoarSize;

            switch (settings.enemyType)
            {
                case < 11:
                    waveRoar.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, settings.enemyType switch
                    {
                        0 => SWITCHES.ENEMY_TYPE.SWITCH.STRIKER,
                        1 => SWITCHES.ENEMY_TYPE.SWITCH.SHOOTER,
                        2 => SWITCHES.ENEMY_TYPE.SWITCH.BIRTHER,
                        3 => SWITCHES.ENEMY_TYPE.SWITCH.SHADOW,
                        4 => SWITCHES.ENEMY_TYPE.SWITCH.TANK,
                        5 => SWITCHES.ENEMY_TYPE.SWITCH.FLYER,
                        6 => SWITCHES.ENEMY_TYPE.SWITCH.IMMORTAL,
                        7 => SWITCHES.ENEMY_TYPE.SWITCH.BULLRUSHER,
                        8 => SWITCHES.ENEMY_TYPE.SWITCH.POUNCER,
                        9 => SWITCHES.ENEMY_TYPE.SWITCH.STRIKER_BERSERK,
                        10 => SWITCHES.ENEMY_TYPE.SWITCH.SHOOTER_SPREAD,
                        _ => 0u
                    });
                    waveRoar.SetSwitch(SWITCHES.ROAR_SIZE.GROUP, size switch
                    {
                        1 => SWITCHES.ROAR_SIZE.SWITCH.SMALL,
                        2 => SWITCHES.ROAR_SIZE.SWITCH.MEDIUM,
                        3 => SWITCHES.ROAR_SIZE.SWITCH.BIG,
                        _ => 0u
                    });
                    waveRoar.SetSwitch(SWITCHES.ENVIROMENT.GROUP, largest.IsOutside.GetValue(settings.isOutside) ? SWITCHES.ENVIROMENT.SWITCH.DESERT : SWITCHES.ENVIROMENT.SWITCH.COMPLEX);
                    waveRoar.PostWithCleanup(EVENTS.PLAY_WAVE_DISTANT_ROAR, settings.position);
                    return false;

                case 12:
                    waveRoar.PostWithCleanup(size < 3 ? EVENTS.DISTANT_ROAR_MEDIUM : EVENTS.DISTANT_ROAR_LARGE, settings.position);
                    return false;

                case 13:
                    waveRoar.PostWithCleanup(largest.SoundID, settings.position);
                    return false;

                case 14 when largest.DynamicSoundIDs.Count is >= 1 and <= 3:
                    waveRoar.PostWithCleanup(largest.DynamicSoundIDs[Math.Clamp(size - 1, 0, largest.DynamicSoundIDs.Count - 1)], settings.position);
                    return false;

                case 11:
                default:
                    return settings.enemyType != 11;
            }
        }

        private static bool TryCondense(byte enemyType, [NotNullWhen(true)] out DistantRoarCustom? largest)
        {
            var filtered = DistantRoarCustom.SharedRoarData.Where(entry => entry.Value.EnemyType == enemyType && entry.Value.IsInWave).ToList();

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
    }
}