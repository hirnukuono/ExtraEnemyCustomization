namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class SpawnWaveAbility : AbilityBase<SpawnWaveBehaviour>
    {
        public uint WaveSettingID { get; set; } = 0u;
        public uint WavePopulationID { get; set; } = 0u;
        public SurvivalWaveSpawnType SpawnType { get; set; } = SurvivalWaveSpawnType.InRelationToClosestAlivePlayer;
        public float SpawnDelay { get; set; } = 0.0f;
        public bool PlayDistantRoar { get; set; } = true;
    }

    public sealed class SpawnWaveBehaviour : AbilityBehaviour<SpawnWaveAbility>
    {
        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => true;
        public override bool IsHostOnlyBehaviour => true;

        protected override void OnEnter()
        {
            Mastermind.Current.TriggerSurvivalWave(Agent.CourseNode,
                Ability.WaveSettingID,
                Ability.WavePopulationID,
                out _,
                Ability.SpawnType,
                Ability.SpawnDelay,
                playScreamOnSpawn: Ability.PlayDistantRoar);
            DoExit();
        }
    }
}