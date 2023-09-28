using BepInEx.Bootstrap;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace AccurateHealthBars
{
    static class RiskOfOptionsCompat
    {
        public static bool Active => Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

        public static void AddOptionEntries(ConfigEntry<int> currentHealthDecimals, ConfigEntry<int> maxHealthDecimals)
        {
            const string MOD_GUID = Main.PluginGUID;
            const string MOD_NAME = "Accurate Health Bars";

            ModSettingsManager.SetModDescription($"Mod settings for {MOD_NAME}", MOD_GUID, MOD_NAME);

            if (currentHealthDecimals != null )
            {
                ModSettingsManager.AddOption(new IntSliderOption(currentHealthDecimals, new IntSliderConfig
                {
                    min = 0,
                    max = 5
                }), MOD_GUID, MOD_NAME);
            }

            if (maxHealthDecimals != null)
            {
                ModSettingsManager.AddOption(new IntSliderOption(maxHealthDecimals, new IntSliderConfig
                {
                    min = 0,
                    max = 5
                }), MOD_GUID, MOD_NAME);
            }
        }
    }
}
