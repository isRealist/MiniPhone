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
            var config = new ModConfig();
            try
            {
                var data = helper.GameContent.Load<Dictionary<string, string>>("Mods/MiniPhone/Config")
                           ?? helper.Data.ReadJson<Dictionary<string, string>>("config.json") ?? new();

                TryParse(data, "ShowHudIcon", v => config.ShowHudIcon = bool.Parse(v));
                TryParse(data, "IconSize", v => config.IconSize = int.Parse(v));
                TryParse(data, "HudOffsetX", v => config.HudOffsetX = int.Parse(v));
                TryParse(data, "HudOffsetY", v => config.HudOffsetY = int.Parse(v));
                TryParse(data, "RandomCallChance", v => config.RandomCallChance = int.Parse(v));
                TryParse(data, "CheckIntervalSeconds", v => config.CheckIntervalSeconds = int.Parse(v));
                TryParse(data, "PlaySound", v => config.PlaySound = bool.Parse(v));
                TryParse(data, "EnableScamCalls", v => config.EnableScamCalls = bool.Parse(v));
                TryParse(data, "ScamCallChance", v => config.ScamCallChance = int.Parse(v));
            }
            catch { }
            return config;
        }

        private static void TryParse(Dictionary<string, string> d, string k, Action<string> a)
        {
            if (d.TryGetValue(k, out var v) && !string.IsNullOrWhiteSpace(v)) a(v);
        }
    }
}