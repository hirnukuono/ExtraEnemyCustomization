using Enemies;

namespace EECustom.Customizations.Detections
{
    public sealed class ScreamingCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        //TODO: Implement me Daddy

        public DetectedBehaviourType DetectedBehaviour { get; set; } = DetectedBehaviourType.DoPropagateScream;
        public bool ImmortalDuringScream { get; set; } = true;

        public override string GetProcessName()
        {
            return "Screaming";
        }

        public void OnSpawned(EnemyAgent agent)
        {
        }
    }

    public enum DetectedBehaviourType
    {
        DoPropagateScream,
        DoScoutScream
    }
}