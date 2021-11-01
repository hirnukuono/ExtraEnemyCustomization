using EECustom.CustomSettings.DTO;
using System;
using System.Collections.Generic;

namespace EECustom.CustomSettings
{
    public static class CustomTentacleManager
    {
        private static readonly Dictionary<int, GPUC_Setup> _tentacleSetups = new();

        public static void GenerateTentacle(CustomTentacle tentInfo)
        {
            if (!IsDefaultType(tentInfo.BodyPrefab))
            {
                return;
            }

            if (!IsDefaultType(tentInfo.BodyMaterial))
            {
                return;
            }

            if (!IsDefaultType(tentInfo.HeadPrefab))
            {
                return;
            }

            if (!IsDefaultType(tentInfo.HeadMaterial))
            {
                return;
            }

            if (!IsDefaultType(tentInfo.Shape))
            {
                return;
            }

            if (_tentacleSetups.ContainsKey(tentInfo.ID))
            {
                return;
            }

            //TODO: For some reason, it's broken. Fix it
            var setup = new GPUC_Setup
            {
                m_bodyPrefab = GetSetup(tentInfo.BodyPrefab).m_bodyPrefab,
                m_bodyTileMaterial = GetSetup(tentInfo.BodyMaterial).m_bodyTileMaterial,
                m_headPrefab = GetSetup(tentInfo.HeadPrefab).m_headPrefab,
                m_headMaterial = GetSetup(tentInfo.HeadMaterial).m_headMaterial
            };
            var baseS = GetSetup(tentInfo.BodyPrefab);
            Logger.Warning($"limit: {baseS.m_globalLimit}, gbuffer: {baseS.m_renderToGBuffer}, segMax: {baseS.m_segmentsMax}");
            setup.m_globalLimit = 50;
            setup.m_renderToGBuffer = true;
            //setup.m_segmentsMax = 14;
            setup.m_shape = GetSetup(tentInfo.Shape).m_shape;
            setup.Setup();

            _tentacleSetups.Add(tentInfo.ID, setup);

            Logger.Debug($"Added Tentacle!: {tentInfo.ID} ({tentInfo.DebugName})");
        }

        public static bool IsDefaultType(GPUCurvyType type)
        {
            return Enum.IsDefined(typeof(GPUCurvyType), type);
        }

        public static GPUC_Setup GetSetup(GPUCurvyType type)
        {
            return GPUCurvyManager.Current.m_setups[(int)type];
        }

        public static GPUC_Setup GetTentacle(int id)
        {
            if (_tentacleSetups.TryGetValue(id, out var setup))
            {
                return setup;
            }
            return null;
        }
    }
}