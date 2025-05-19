using Agents;
using EEC.Managers;
using EEC.Utils.Json.Elements;
using Enemies;
using System.Text.Json.Serialization;
using UnityEngine;
using SWITCH_ID = AK.SWITCHES.ENEMY_TYPE.SWITCH;

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
        Custom,
        CustomDynamic
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
        [JsonIgnore]
        public static Dictionary<uint, RoarData> SharedRoarData { get; internal set; } = new();
        public uint SoundID { get; set; } = 0u;
        public float Interval { get; set; } = 0.0f;
        public bool OnlyForSurvivalWave { get; set; } = true;
        public bool IsGlobal { get; set; } = true;
        public bool OnlyUseOverrides { get; set; } = false;
        public RoarSoundOverride RoarSound { get; set; } = RoarSoundOverride.Striker;
        public List<uint> DynamicSoundIDs { get; set; } = new();
        public RoarSizeOverride RoarSize { get; set; } = RoarSizeOverride.Unchanged;
        public BoolBase IsOutside { get; set; } = BoolBase.Unchanged;

        private float _timer = 0.0f;
        private CellSoundPlayer? _soundPlayer;
        private static readonly Dictionary<string, uint> _waveRoars = new()
        {
            { "Striker", SWITCH_ID.STRIKER },
            { "Shooter", SWITCH_ID.SHOOTER },
            { "Bullrush", SWITCH_ID.BULLRUSHER },
            { "Shadow", SWITCH_ID.SHADOW },
            { "Flyer", SWITCH_ID.FLYER },
            { "Tank", SWITCH_ID.TANK },
            { "Birther", SWITCH_ID.BIRTHER },
            { "Pouncer", SWITCH_ID.POUNCER },
            { "Immortal", SWITCH_ID.IMMORTAL },
            { "Striker_Berserk", SWITCH_ID.STRIKER_BERSERK },
            { "Shooter_Spread", SWITCH_ID.SHOOTER_SPREAD },
            { "None", 11u },
            { "OldDistantRoar", 12u },
            { "Custom", 13u },
            { "CustomDynamic", 14u }
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
            SharedRoarData.Clear();

            if (ConfigManager.Global.AddUnusedVanillaRoars)
            {
                SharedRoarData = new()
                {
                    { 46u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Pouncer, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.POUNCER, EnemyType = 8 } },
                    { 47u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Immortal, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.IMMORTAL, EnemyType = 6 } },
                    { 55u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Birther, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.BIRTHER, EnemyType = 2 } },
                    { 62u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Striker_Berserk, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.STRIKER_BERSERK, EnemyType = 9 } },
                    { 63u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Striker_Berserk, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.STRIKER_BERSERK, EnemyType = 9 } },
                    { 52u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Shooter_Spread, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.SHOOTER_SPREAD, EnemyType = 10 } },
                    { 56u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.Shooter_Spread, OnlyUseOverrides = true }, SwitchID = SWITCH_ID.SHOOTER_SPREAD, EnemyType = 10 } },
                    { 44u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.None, OnlyUseOverrides = true }, SwitchID = 11u, EnemyType = 11 } },
                    { 61u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.None, OnlyUseOverrides = true }, SwitchID = 11u, EnemyType = 11 } },
                    { 58u, new() { RoarSettings = new() { RoarSound = RoarSoundOverride.None, OnlyUseOverrides = true }, SwitchID = 11u, EnemyType = 11 } }
                };
            }
        }

        public override void OnTargetIDLookupBuilt()
        {
            foreach (uint target in TargetEnemyIDs)
            {
                if (_waveRoars.TryGetValue(RoarSound.ToString(), out uint switchID))
                {
                    SharedRoarData[target] = new() { RoarSettings = this, SwitchID = switchID, EnemyType = (byte)RoarSound };
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

            SharedRoarData.Clear();
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
        
        public class RoarData
        {
            public DistantRoarCustom RoarSettings { get; set; } = new();
            public uint SwitchID { get; set; } = 0u;
            public byte EnemyType { get; set; } = 0;
            public bool IsInWave { get; set; } = false;
        }
    }
}
