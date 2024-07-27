using Agents;
using EEC.Managers;
using EEC.Utils.Json.Elements;
using Enemies;
using ExtraEnemyCustomization.EnemyCustomizations.Properties.Inject;
using UnityEngine;
using SwitchID = AK.SWITCHES.ENEMY_TYPE.SWITCH;

namespace EEC.EnemyCustomizations.Properties
{
    public enum RoarSoundOverride : byte
    {       
        Striker,
        Shooter,
        Birther,
        Shadow,
        Tank,
        Flyer,
        Immortal,
        Bullrush,
        Pouncer,
        Striker_Berserk,
        Shooter_Spread, 
        None,
        OldDistantRoar,
        Custom
    }
    public enum RoarSizeOverride : byte
    {
        Unchanged,
        Small,
        Medium,
        Big
    }

    public sealed class DistantRoarCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public uint SoundID { get; set; } = 0u;
        public float Interval { get; set; } = 0.0f;
        public bool OnlyForSurvivalWave { get; set; } = true;
        public bool IsGlobal { get; set; } = true;
        public bool OnlyUseOverrides { get; set; } = false;
        public RoarSoundOverride RoarSound { get; set; } = RoarSoundOverride.Striker;
        public RoarSizeOverride RoarSize { get; set; } = RoarSizeOverride.Unchanged;
        public BoolBase IsOutside { get; set; } = BoolBase.Unchanged;

        private float _timer = 0.0f;
        private CellSoundPlayer? _soundPlayer;
        private static readonly Dictionary<string, uint> _waveRoars = new()
        {
            { "Striker", SwitchID.STRIKER },
            { "Shooter", SwitchID.SHOOTER },
            { "Bullrush", SwitchID.BULLRUSHER },
            { "Shadow", SwitchID.SHADOW },
            { "Flyer", SwitchID.FLYER },
            { "Tank", SwitchID.TANK },
            { "Birther", SwitchID.BIRTHER },
            { "Pouncer", SwitchID.POUNCER },
            { "Immortal", SwitchID.IMMORTAL },
            { "Striker_Berserk", SwitchID.STRIKER_BERSERK },
            { "Shooter_Spread", SwitchID.SHOOTER_SPREAD },
            { "None", 99900u },
            { "OldDistantRoar", 99901u },
            { "Custom", 99902u }
        };

        public override string GetProcessName()
        {
            return "DistantRoar";
        }

        public override void OnAssetLoaded()
        {
            _soundPlayer = new(Vector3.zero);
        }

        public override void OnConfigLoaded()
        {
            base.OnConfigLoaded();
            SharedRoarData.Dict.Clear();

            if (ConfigManager.Global.AddUnusedVanillaRoars)
            {
                SharedRoarData.Dict.TryAdd(46u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Pouncer }, SwitchID = SwitchID.POUNCER, EnemyType = 8 });
                SharedRoarData.Dict.TryAdd(47u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Immortal }, SwitchID = SwitchID.IMMORTAL, EnemyType = 6 });
                SharedRoarData.Dict.TryAdd(55u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Birther }, SwitchID = SwitchID.BIRTHER, EnemyType = 2 });
                SharedRoarData.Dict.TryAdd(53u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Striker_Berserk }, SwitchID = SwitchID.STRIKER_BERSERK, EnemyType = 9 });
                SharedRoarData.Dict.TryAdd(62u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Striker_Berserk }, SwitchID = SwitchID.STRIKER_BERSERK, EnemyType = 9 });
                SharedRoarData.Dict.TryAdd(63u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Striker_Berserk }, SwitchID = SwitchID.STRIKER_BERSERK, EnemyType = 9 });
                SharedRoarData.Dict.TryAdd(52U, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Shooter_Spread }, SwitchID = SwitchID.SHOOTER_SPREAD, EnemyType = 10 });
                SharedRoarData.Dict.TryAdd(56U, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Shooter_Spread }, SwitchID = SwitchID.SHOOTER_SPREAD, EnemyType = 10 });
            } 
        }

        public override void OnTargetIDLookupBuilt()
        {
            foreach (uint target in TargetEnemyIDs)
            {
                if (_waveRoars.TryGetValue(RoarSound.ToString(), out uint switchID))
                {
                    if (!SharedRoarData.Dict.ContainsKey(target))
                        SharedRoarData.Dict.TryAdd(target, new() { RoarSettings = this, SwitchID = switchID, EnemyType = ((byte)RoarSound) });
                    else
                        SharedRoarData.Dict[target] = new() { RoarSettings = this, SwitchID = switchID, EnemyType = ((byte)RoarSound) };
                }
            }

            /*Debug.LogError("Dictionary contents:");
            foreach (var kvp in SharedRoarData.Dict)
            {
                Debug.LogError($"Key: {kvp.Key}");

                var roarData = kvp.Value;
                Debug.LogError($"  SwitchID: {roarData.SwitchID}");
                Debug.LogError($"  SwitchID: {roarData.EnemyType}");
            }*/
        }

        public override void OnConfigUnloaded()
        {
            _soundPlayer?.Recycle();
            _soundPlayer = null;

            SharedRoarData.Dict.Clear();
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (OnlyUseOverrides)
                return;

            if (!agent.TryGetSpawnData(out var spawnData))
                return;

            if (spawnData.mode != AgentMode.Agressive)
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
