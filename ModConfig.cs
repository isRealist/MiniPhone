using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace MiniPhone.Config
{
    public class ModConfig
    {
        public bool ShowHudIcon { get; set; } = true;
        public int IconSize { get; set; } = 16;
        public int HudOffsetX { get; set; } = -20;
        public int HudOffsetY { get; set; } = 20;

        public int RandomCallChance { get; set; } = 2;
        public int CheckIntervalSeconds { get; set; } = 5;

        public bool PlaySound { get; set; } = true;

        public bool EnableScamCalls { get; set; } = true;
        public int ScamCallChance { get; set; } = 30;

        public static ModConfig Load(IModHelper helper)
        {
            ModConfig config = helper.ReadConfig<ModConfig>() ?? new ModConfig();

            try
            {
                var legacy = helper.GameContent.Load<Dictionary<string, string>>("Mods/MiniPhone/Config");
                if (legacy != null)
                    MergeFromDictionary(config, legacy);
            }
            catch
            {
            }

            try
            {
                helper.Data.WriteJsonFile("config.json", config);
            }
            catch
            {
            }

            return config;
        }

        private static void MergeFromDictionary(ModConfig config, Dictionary<string, string> d)
        {
            TryParse(d, "ShowHudIcon", v => config.ShowHudIcon = ParseBoolFallback(v, config.ShowHudIcon));
            TryParse(d, "IconSize", v => config.IconSize = ParseIntFallback(v, config.IconSize));
            TryParse(d, "HudOffsetX", v => config.HudOffsetX = ParseIntFallback(v, config.HudOffsetX));
            TryParse(d, "HudOffsetY", v => config.HudOffsetY = ParseIntFallback(v, config.HudOffsetY));
            TryParse(d, "RandomCallChance", v => config.RandomCallChance = ParseIntFallback(v, config.RandomCallChance));
            TryParse(d, "CheckIntervalSeconds", v => config.CheckIntervalSeconds = ParseIntFallback(v, config.CheckIntervalSeconds));
            TryParse(d, "PlaySound", v => config.PlaySound = ParseBoolFallback(v, config.PlaySound));
            TryParse(d, "EnableScamCalls", v => config.EnableScamCalls = ParseBoolFallback(v, config.EnableScamCalls));
            TryParse(d, "ScamCallChance", v => config.ScamCallChance = ParseIntFallback(v, config.ScamCallChance));
        }

        private static void TryParse(Dictionary<string, string> d, string k, Action<string> apply)
        {
            if (d.TryGetValue(k, out var v) && !string.IsNullOrWhiteSpace(v))
                apply(v);
        }

        private static int ParseIntFallback(string s, int fallback)
        {
            return int.TryParse(s, out var i) ? i : fallback;
        }

        private static bool ParseBoolFallback(string s, bool fallback)
        {
            if (bool.TryParse(s, out var b)) return b;
            s = s?.Trim().ToLowerInvariant();
            if (s == "1" || s == "yes" || s == "y" || s == "true" || s == "on") return true;
            if (s == "0" || s == "no" || s == "n" || s == "false" || s == "off") return false;
            return fallback;
        }
    }
}

