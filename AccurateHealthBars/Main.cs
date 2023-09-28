using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using System.Diagnostics;

namespace AccurateHealthBars
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "AccurateHealthBars";
        public const string PluginVersion = "1.0.0";
        
        internal static ConfigEntry<int> CurrentHealthDecimals { get; private set; }
        internal static ConfigEntry<int> MaxHealthDecimals { get; private set; }

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Log.Init(Logger);

            CurrentHealthDecimals = Config.Bind("General", "Current Health Decimals", 2);
            MaxHealthDecimals = Config.Bind("General", "Max Health Decimals", 2);

            if (RiskOfOptionsCompat.Active)
            {
                RiskOfOptionsCompat.AddOptionEntries(CurrentHealthDecimals, MaxHealthDecimals);
            }

            HealthBarPatches.Init();

            stopwatch.Stop();
            Log.Info_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        }
    }
}
