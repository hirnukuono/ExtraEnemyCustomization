using AK;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using EEC.EnemyCustomizations.Properties;
using HarmonyLib;
using LevelGeneration;
using System.Collections;
using UnityEngine;

namespace ExtraEnemyCustomization.EnemyCustomizations.Properties.Inject
{ 
    [HarmonyPatch(typeof(LG_LevelInteractionManager), nameof(LG_LevelInteractionManager.DoSetWaveRoarSoundInformation))]
    internal static class Inject_DoSetWaveRoar
    {
        [HarmonyWrapSafe]
        public static bool Prefix(LG_LevelInteractionManager.pWaveRoarSettings settings)
        {
            DistantRoarCustom? largest = SharedRoarData.Condense(SharedRoarData.Dict.Where(entry => entry.Value.EnemyType == settings.enemyType && entry.Value.IsInWave).ToList());
            if (largest == null)
                return true;
            
            CellSoundPlayer csPlayer = new(Vector3.zero);
            csPlayer.UpdatePosition(settings.position);
            bool useOldRoar = false;
            CoroutineManager.StartCoroutine(Cleanup(csPlayer).WrapToIl2Cpp());

            switch (settings.enemyType)
            {
                case 0:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.STRIKER);
                    break;
                case 1:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.SHOOTER);
                    break;
                case 2:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.BIRTHER);
                    break;
                case 3:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.SHADOW);
                    break;
                case 4:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.TANK);
                    break;
                case 5:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.FLYER);
                    break;
                case 6:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.IMMORTAL);
                    break;
                case 7:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.BULLRUSHER);
                    break;
                case 8:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.POUNCER);
                    break;
                case 9:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.STRIKER_BERSERK);
                    break;
                case 10:
                    csPlayer.SetSwitch(SWITCHES.ENEMY_TYPE.GROUP, SWITCHES.ENEMY_TYPE.SWITCH.SHOOTER_SPREAD);
                    break;
                case 11:
                    return false;
                case 12:
                    useOldRoar = true;
                    break;
                case 13:
                    csPlayer.Post(largest.SoundID, largest.IsGlobal);
                    return false;
                default:
                    return true;
            }

            int size = (int)largest.RoarSize == 0 ? settings.roarSize : (int)largest.RoarSize;
            switch (size)
            {
                case 1:
                    csPlayer.SetSwitch(SWITCHES.ROAR_SIZE.GROUP, SWITCHES.ROAR_SIZE.SWITCH.SMALL);
                    if (useOldRoar)
                    {
                        csPlayer.Post(EVENTS.DISTANT_ROAR_MEDIUM, largest.IsGlobal);
                        return false;
                    }
                    break;
                case 2:
                    csPlayer.SetSwitch(SWITCHES.ROAR_SIZE.GROUP, SWITCHES.ROAR_SIZE.SWITCH.MEDIUM);
                    if (useOldRoar)
                    {
                        csPlayer.Post(EVENTS.DISTANT_ROAR_MEDIUM, largest.IsGlobal);
                        return false;
                    }
                    break;
                case 3:
                    csPlayer.SetSwitch(SWITCHES.ROAR_SIZE.GROUP, SWITCHES.ROAR_SIZE.SWITCH.BIG);
                    if (useOldRoar)
                    {
                        csPlayer.Post(EVENTS.DISTANT_ROAR_LARGE, largest.IsGlobal);
                        return false;
                    }
                    break;
            }

            csPlayer.SetSwitch(SWITCHES.ENVIROMENT.GROUP, largest.IsOutside.GetValue(settings.isOutside) ? SWITCHES.ENVIROMENT.SWITCH.DESERT : SWITCHES.ENVIROMENT.SWITCH.COMPLEX);
            csPlayer.Post(EVENTS.PLAY_WAVE_DISTANT_ROAR, true);
            return false;
        }

        static IEnumerator Cleanup(CellSoundPlayer csPlayer)
        {
            yield return new WaitForSeconds(10f);
            csPlayer.Recycle();
        }
    }
}