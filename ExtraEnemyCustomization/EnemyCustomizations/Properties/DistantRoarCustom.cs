using Agents;
using Enemies;
using UnityEngine;

namespace EECustom.EnemyCustomizations.Properties
{
    public sealed class DistantRoarCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public uint SoundID { get; set; } = 0u;
        public float Interval { get; set; } = 0.0f;
        public bool OnlyForSurvivalWave { get; set; } = true;
        public bool IsGlobal { get; set; } = true;

        private float _timer = 0.0f;
        private CellSoundPlayer _soundPlayer;

        public override string GetProcessName()
        {
            return "DistantRoar";
        }

        public override void OnAssetLoaded()
        {
            _soundPlayer = new(Vector3.zero);
        }

        public override void OnConfigUnloaded()
        {
            _soundPlayer?.Recycle();
            _soundPlayer = null;
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (agent.GetSpawnData().mode != AgentMode.Agressive)
                return;

            if (OnlyForSurvivalWave)
            {
                if (!agent.TryGetEnemyGroup(out var group))
                    return;

                if (group.SurvivalWave == null)
                    return;
            }

            if (_timer <= Clock.Time)
            {
                if (_soundPlayer != null)
                {
                    _soundPlayer.UpdatePosition(agent.Position);
                    _soundPlayer.Post(SoundID, IsGlobal);
                }
                _timer = Clock.Time + Interval;
            }
        }
    }
}