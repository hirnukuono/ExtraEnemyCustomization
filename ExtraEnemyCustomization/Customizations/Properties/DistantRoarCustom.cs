using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Properties
{
    public sealed class DistantRoarCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public uint SoundID { get; set; } = 0u;
        public float Interval { get; set; } = 0.0f;

        private float _timer = 0.0f;
        private CellSoundPlayer _soundPlayer;

        public override string GetProcessName()
        {
            return "DistantRoar";
        }

        public override void OnConfigLoaded()
        {
            _soundPlayer = new();
        }

        public override void OnConfigUnloaded()
        {
            _soundPlayer?.Recycle();
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (_timer <= Clock.Time)
            {
                if (_soundPlayer != null)
                {
                    _soundPlayer.UpdatePosition(agent.Position);
                    _soundPlayer.Post(SoundID, isGlobal: true);
                }
                _timer = Clock.Time + Interval;
            }
        }
    }
}
