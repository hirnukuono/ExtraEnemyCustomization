using UnityEngine;
using AkEventCallback = AkCallbackManager.EventCallback;

namespace ExtraEnemyCustomization.Extensions
{
    public static class CellSoundPlayerExtension
    {
        public static uint PostWithCleanup(this CellSoundPlayer soundPlayer, uint eventID, Vector3 pos)
        {
            return soundPlayer.Post(eventID, pos, 1u, (AkEventCallback)SoundDoneCallback, soundPlayer);
        }

        public static void SoundDoneCallback(Il2CppSystem.Object in_cookie, AkCallbackType in_type, AkCallbackInfo callbackInfo)
        {
            var callbackPlayer = in_cookie.Cast<CellSoundPlayer>();
            callbackPlayer?.Recycle();
        }
    }
}
