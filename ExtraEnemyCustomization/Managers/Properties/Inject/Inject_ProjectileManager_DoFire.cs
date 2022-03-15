using EECustom.Events;
using Enemies;
using HarmonyLib;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EECustom.Managers.Properties.Inject
{
    [HarmonyPatch(typeof(ProjectileManager), nameof(ProjectileManager.DoFireTargeting))]
    internal static class Inject_ProjectileManager_DoFire
    {
        [HarmonyWrapSafe]
        [SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "IDamagable IS Unity Component Interface")]
        internal static void Postfix(ProjectileManager.pFireTargeting data)
        {
            if (data.burstSize == 1)
                return;

            if (data.burstSize > ushort.MaxValue)
                return;

            if (data.burstSize < ushort.MinValue)
                return;

            var tempGO = ProjectileManager.s_tempGO;
            if (tempGO == null)
                return;

            var instanceID = tempGO.GetInstanceID();
            if (tempGO.GetComponent<IProjectile>() == null)
                return;

            if (!SNet_Replication.TryGetReplicator(checked((ushort)data.burstSize), out var replicator))
                return;

            if (replicator?.ReplicatorSupplier == null)
                return;

            var enemySync = replicator.ReplicatorSupplier.TryCast<EnemySync>();
            if (enemySync != null)
            {
                ProjectileOwnerManager.Set(instanceID, enemySync.m_agent);

                MonoBehaviourEventHandler.AttatchToObject(tempGO, onDestroyed: (_) =>
                {
                    ProjectileOwnerManager.Remove(instanceID);
                });
            }
        }
    }
}
