using EECustom.Utils;
using Enemies;

namespace EECustom.Customizations.Detections
{
    public sealed class ScoutAnimCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        internal static readonly ScoutAnimSync _animSync = new();

        public AnimationRandomType RandomType { get; set; } = AnimationRandomType.PerDetection;
        public EnemyAnimType BendAnimation { get; set; } = EnemyAnimType.AbilityUseOut;
        public EnemyAnimType StandAnimation { get; set; } = EnemyAnimType.AbilityUse;
        public float ChanceToBend { get; set; } = 1.0f;

        public bool OverridePullingAnimation { get; set; } = false;
        public EnemyAnimType PullingAnimation { get; set; } = EnemyAnimType.AbilityUseOut;

        static ScoutAnimCustom()
        {
            _animSync.Setup();
        }

        public override string GetProcessName()
        {
            return "ScoutAnim";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var data = agent.RegisterOrGetProperty<ScoutAnimOverrideProperty>();
            data.Agent = agent;
            switch (RandomType)
            {
                case AnimationRandomType.PerEnemy:
                    data.ChanceToBend = Rand.CanDoBy(ChanceToBend) ? 1.0f : 0.0f;
                    break;

                case AnimationRandomType.PerDetection:
                    data.ChanceToBend = ChanceToBend;
                    break;
            }

            data.BendAnimation = BendAnimation;
            data.StandAnimation = StandAnimation;
            data.OverridePullingAnimation = OverridePullingAnimation;
            data.PullingAnimation = PullingAnimation;
        }

        public enum AnimationRandomType
        {
            PerEnemy,
            PerDetection
        }
    }
}