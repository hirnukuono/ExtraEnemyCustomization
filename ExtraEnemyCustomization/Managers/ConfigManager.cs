using EEC.Configs;
using EEC.Configs.Customizations;
using EEC.Utils.Integrations;
using GTFO.API.Utilities;
using System.Reflection;

namespace EEC.Managers
{
    public static partial class ConfigManager
    {
        public static bool UseLiveEdit { get; private set; }
        public static bool LinkMTFOHotReload { get; private set; }
        public static string BasePath => EntryPoint.BasePath;

        [ConfigCache] public static GlobalConfig Global { get; private set; } = new();
        [ConfigCache] public static CategoryConfig Categories { get; private set; } = new();
        [ConfigCache] public static ModelCustomConfig ModelCustom { get; private set; } = new();
        [ConfigCache] public static AbilityCustomConfig AbilityCustom { get; private set; } = new();
        [ConfigCache] public static ProjectileCustomConfig ProjectileCustom { get; private set; } = new();
        [ConfigCache] public static TentacleCustomConfig TentacleCustom { get; private set; } = new();
        [ConfigCache] public static DetectionCustomConfig DetectionCustom { get; private set; } = new();
        [ConfigCache] public static PropertyCustomConfig PropertyCustom { get; private set; } = new();
        [ConfigCache] public static SpawnCostCustomConfig SpawnCostCustom { get; private set; } = new();
        [ConfigCache] public static EnemyAbilityCustomConfig EnemyAbilityCustom { get; private set; } = new();

        private static readonly Type[] _configTypes = new Type[]
        {
            //Normal Configs
            typeof(GlobalConfig),
            typeof(CategoryConfig),
            typeof(ScoutWaveConfig),

            //Customization Configs
            typeof(AbilityCustomConfig),
            typeof(DetectionCustomConfig),
            typeof(EnemyAbilityCustomConfig),
            typeof(ModelCustomConfig),
            typeof(ProjectileCustomConfig),
            typeof(PropertyCustomConfig),
            typeof(SpawnCostCustomConfig),
            typeof(TentacleCustomConfig)
        };

        private static readonly Dictionary<Type, string> _configTypeToFileName = new();
        private static readonly Dictionary<string, Type> _configFileNameToType = new();
        private static readonly Dictionary<string, Config> _configInstances = new();
        private static readonly IEnumerable<PropertyInfo> _cacheProperties;
        private static LiveEditListener _liveEdit;

        static ConfigManager()
        {
            _cacheProperties = typeof(ConfigManager)
                .GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(ConfigCacheAttribute)));
        }

        internal static void Initialize()
        {
            UseLiveEdit = Configuration.UseLiveEdit;
            LinkMTFOHotReload = Configuration.LinkMTFOHotReload;

            foreach (var configType in _configTypes)
            {
                var instance = Activator.CreateInstance(configType) as Config;
                var fileName = instance.FileName;
                _configTypeToFileName[configType] = fileName;
                _configFileNameToType[fileName] = configType;
                _configFileNameToType[fileName.ToLowerInvariant()] = configType; //For Easier Access
                _configInstances[fileName] = instance;
            }

            LoadAllConfig();
            GenerateBuffer();

            if (LinkMTFOHotReload)
            {
                MTFOUtil.HotReloaded += ReloadConfig;
            }

            if (UseLiveEdit)
            {
                SetupLiveEdit();
            }

            Configuration.OnReload += OnConfigReloaded;
        }

        internal static void OnConfigReloaded()
        {
            if (LinkMTFOHotReload != Configuration.LinkMTFOHotReload)
            {
                if (LinkMTFOHotReload)
                    MTFOUtil.HotReloaded -= ReloadConfig;
                else
                    MTFOUtil.HotReloaded += ReloadConfig;
                LinkMTFOHotReload = !LinkMTFOHotReload;
            }

            if (UseLiveEdit != Configuration.UseLiveEdit)
            {
                if (UseLiveEdit)
                    _liveEdit?.Dispose();
                else
                    SetupLiveEdit();
                UseLiveEdit = !UseLiveEdit;
            }
        }

        internal static void FireAssetLoaded()
        {
            foreach (var config in _customizationBuffer)
            {
                config.OnAssetLoaded();
            }
        }

        private static void SetupLiveEdit()
        {
            if (Directory.Exists(BasePath))
            {
                _liveEdit = LiveEdit.CreateListener(BasePath, "*.*", true);
                _liveEdit.FileChanged += LiveEdit_FileChanged;
                _liveEdit.StartListen();
            }
        }
    }
}