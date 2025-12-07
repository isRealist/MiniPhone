using MiniPhone.Calls;
using MiniPhone.Config;
using MiniPhone.Furniture;
using MiniPhone.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MiniPhone
{
    public class MiniPhoneMod : Mod
    {
        internal static MiniPhoneMod Instance { get; private set; } = null!;
        internal ModConfig Config { get; private set; } = null!;
        internal CallManager Calls { get; private set; } = null!;
        internal PhoneHudElement Hud { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = ModConfig.Load(helper);

            Calls = new CallManager(this);
            Hud = new PhoneHudElement(this);
            new InventoryPhone(this);
            new FurniturePatch();

            helper.Events.GameLoop.SaveLoaded += (_, __) => MiniPhoneFurniture.Register(helper);
            helper.Events.Content.AssetRequested += Calls.OnAssetRequested;

            Monitor.Log($"MiniPhone v2.1 by justcallmeR loaded! | {Config.RandomCallChance}% every {Config.CheckIntervalSeconds}s", LogLevel.Info);
        }
    }
}