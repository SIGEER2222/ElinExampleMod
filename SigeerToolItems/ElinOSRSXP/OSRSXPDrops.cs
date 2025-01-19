using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ElinOSRSXP {
    [BepInPlugin("nguyen.elin.plugins.XPDrops", "OSRS XP Drops", "1.0.0")]
    public class OSRSXPDrops : BaseUnityPlugin {
        private void Awake() {
            Logger.LogInfo("OSRS XP Drops Plugin Initialized!");
            xpThresh = Config.Bind("General", "Notification Threshold", 0.1f, new ConfigDescription("Intensity of the feature.", new AcceptableValueRange<float>(0.01f, 100f)));
            fixedDefault = Config.Bind("General", "Fixed Default", false, new ConfigDescription("Indicates if the default threshold value has been updated."));

            if (!fixedDefault.Value && xpThresh.Value == 0.1f) {
                xpThresh.Value = 0.01f;
                Logger.LogInfo("Updated 'Notification Threshold' to new default value: 0.01f");
                fixedDefault.Value = true;
                Logger.LogInfo("Marked default as fixed.");
            }

            new Harmony("nguyen.elin.plugins.XPDrops").PatchAll();
        }

        public static ConfigEntry<float> xpThresh;
        public static ConfigEntry<bool> fixedDefault;
    }
}