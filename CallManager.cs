using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MiniPhone.Calls
{
    internal class CallManager
    {
        private readonly MiniPhoneMod mod;

        public CallManager(MiniPhoneMod mod) => this.mod = mod;

        public void TriggerRandomCall()
        {
            var npcs = Utility.getAllCharacters()
                .Where(n => n.isVillager() && !n.isInvisible && n.canTalk())
                .ToList();

            string dialogue = npcs.Any() && !(mod.Config.EnableScamCalls && Game1.random.Next(100) < mod.Config.ScamCallChance)
                ? mod.Helper.Translation.Get("call.npc")
                    .ToString()
                    .Replace("{{NPC}}", npcs[Game1.random.Next(npcs.Count)].displayName)
                    .Replace("{{Player}}", Game1.player.Name)
                : mod.Helper.Translation.Get("call.scam").ToString();

            if (mod.Config.PlaySound) Game1.playSound("phone");

            try
            {
                AccessTools.Method(typeof(StardewValley.Objects.Phone), "Ring")
                    ?.Invoke(null, new object[] { "mod.miniphone", dialogue });
            }
            catch
            {
                Game1.addHUDMessage(new HUDMessage("Phone rang!", 2));
            }
        }

        public void OnAssetRequested(object? sender, AssetRequestedEventArgs e) { }
    }
}