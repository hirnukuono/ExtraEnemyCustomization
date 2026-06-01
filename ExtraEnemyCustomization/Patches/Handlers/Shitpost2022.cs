using EEC.EnemyCustomizations.Models.Handlers;
using EEC.Events;
using EEC.Utils.Unity;
using Enemies;
using System.Collections;
using UnityEngine;

namespace EEC.Patches.Handlers.Yes.Yes.Yes.Yes
{
    [CallConstructorOnLoad]
    public static class Shitpost2022
    {
        static Shitpost2022()
        {
            if (Configuration.CanShitpostOf(ShitpostType.Dinnerbone))
            {
                EnemyEvents.Spawned += EnemyEvents_Spawned;
            }
        }

        private static void EnemyEvents_Spawned(EnemyAgent agent)
        {
            var marker = GuiManager.NavMarkerLayer.PlaceCustomMarker(NavMarkerOption.Title, agent.ModelRef.m_markerTagAlign, "<alpha=#44>Dinnerbone", 0f, false);
            marker.SetVisualStates(NavMarkerOption.Empty, NavMarkerOption.Empty, NavMarkerOption.Empty, NavMarkerOption.Empty);
            marker.m_titleSubObj.SetEnabled(false);
            marker.SetPinEnabled(false);
            agent.AI.StartCoroutine(UpdateMarker(agent, marker));
            agent.AddOnDeadOnce(() =>
            {
                GuiManager.NavMarkerLayer.RemoveMarker(marker);
            });

            var hip = agent.Anim.GetBoneTransform(HumanBodyBones.Hips);
            if (hip != null)
            {
                var boneOffset = hip.gameObject.AddComponent<BoneOffsetHandler>();
                boneOffset.Animator = agent.Anim;
                boneOffset.RotationOffset = new Vector3(0f, 180f, 0f);
            }
            else
            {
                agent.MainModelTransform.Rotate(Vector3.forward, 180f);
            }
        }

        private static IEnumerator UpdateMarker(EnemyAgent agent, NavMarker marker)
        {
            var yielder = WaitFor.Seconds[0.25f];
            var enabled = false;

            yield return yielder;
            while (agent.Alive)
            {
                if (marker.m_currentState == NavMarkerState.InFocus)
                {
                    var distanceSqr = (agent.Position - CameraManager.GetCurrentCamera().transform.position).sqrMagnitude;
                    if (!enabled && distanceSqr <= 64f)
                    {
                        marker.m_titleSubObj.SetEnabled(true);
                        enabled = true;
                    }
                    else if (enabled && distanceSqr > 64f)
                    {
                        marker.m_titleSubObj.SetEnabled(false);
                        enabled = false;
                    }
                }
                else if (enabled)
                {
                    marker.m_titleSubObj.SetEnabled(false);
                    enabled = false;
                }

                yield return yielder;
            }
        }
    }
}
