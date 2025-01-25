using AK;
using HarmonyLib;
using LevelGeneration;
using AkEventCallback = AkCallbackManager.EventCallback;

namespace EEC.EnemyCustomizations.Properties.Inject
{ 
    [HarmonyPatch]
    internal static class Inject_DoSetWaveRoar
    {
        [HarmonyPatch(typeof(LG_LevelInteractionManager), nameof(LG_LevelInteractionManager.DoSetWaveRoarSoundInformation))]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        private static bool Pre_DoSetWaveRoarSound(LG_LevelInteractionManager.pWaveRoarSettings settings)
        {
            if (!SharedRoarData.TryCondense(settings.enemyType, out var largest))
            {
                return true;
            }

            CellSoundPlayer waveRoar = new();
            int size = largest.RoarSize == 0 ? settings.roarSize : (int)largest.RoarSize;

            if (settings.enemyType < 11)
            {
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
                    _ => SWITCHES.ENEMY_TYPE.SWITCH.STRIKER
                });
            }
            else if (settings.enemyType == 12)
            {
                waveRoar.Post(size < 3 ? EVENTS.DISTANT_ROAR_MEDIUM : EVENTS.DISTANT_ROAR_LARGE, settings.position, 1u, (AkEventCallback)SoundDoneCallback, waveRoar);
                return false;
            }
            else if (settings.enemyType == 13)
            {
                waveRoar.Post(largest.SoundID, settings.position, 1u, (AkEventCallback)SoundDoneCallback, waveRoar);
                return false;
            }
            else
            {
                return settings.enemyType != 11;
            }

            waveRoar.SetSwitch(SWITCHES.ROAR_SIZE.GROUP, size switch
            {
                1 => SWITCHES.ROAR_SIZE.SWITCH.SMALL,
                2 => SWITCHES.ROAR_SIZE.SWITCH.MEDIUM,
                3 => SWITCHES.ROAR_SIZE.SWITCH.BIG,
                _ => SWITCHES.ROAR_SIZE.SWITCH.SMALL
            });

            waveRoar.SetSwitch(SWITCHES.ENVIROMENT.GROUP, largest.IsOutside.GetValue(settings.isOutside) ? SWITCHES.ENVIROMENT.SWITCH.DESERT : SWITCHES.ENVIROMENT.SWITCH.COMPLEX);
            waveRoar.Post(EVENTS.PLAY_WAVE_DISTANT_ROAR, settings.position, 1u, (AkEventCallback)SoundDoneCallback, waveRoar);
            return false;
        }

        private static void SoundDoneCallback(Il2CppSystem.Object in_cookie, AkCallbackType in_type, AkCallbackInfo callbackInfo)
        {
            var callbackPlayer = in_cookie.Cast<CellSoundPlayer>();
            callbackPlayer?.Recycle();
        }
    }
}