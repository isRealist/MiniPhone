using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MiniPhone.Calls
{
    internal class CallManager
    {
        private readonly MiniPhoneMod mod;
        public CallManager(MiniPhoneMod mod) => this.mod = mod;

        public void ShowCallMenu()
        {
            var npcs = Utility.getAllCharacters()
                .Where(n => n.isVillager() && !n.isInvisible && n.canTalk())
                .OrderBy(n => n.displayName)
                .ToList();

            if (!npcs.Any())
            {
                Game1.showRedMessage("No one to call!");
                return;
            }

            var responses = npcs
                .Select(n => new Response(n.Name, n.displayName))
                .ToList();

            responses.Add(new Response("cancel", "Hang up"));

            Game1.currentLocation.createQuestionDialogue(
                "Who would you like to call?",
                responses.ToArray(),
                (Farmer who, string answer) =>
                {
                    if (answer == "cancel")
                        return;

                    var npc = npcs.FirstOrDefault(n => n.Name == answer);
                    if (npc != null)
                        TriggerCall(npc, true);
                }
            );
        }

        public void TriggerCall(NPC npc, bool isManual)
        {
            string prefix = isManual ? "call.manual." : "call.random.";
            string key = prefix + npc.Name;

            var custom = mod.Helper.Translation.Get(key);
            string dialogText =
                !string.IsNullOrWhiteSpace(custom.ToString()) && !custom.ToString().Contains("{{")
                ? custom.ToString()
                : mod.Helper.Translation.Get("call.npc")
                    .ToString()
                    .Replace("{{NPC}}", npc.displayName)
                    .Replace("{{Player}}", Game1.player.Name);

            if (!isManual && mod.Config.EnableScamCalls && Game1.random.Next(100) < mod.Config.ScamCallChance)
                dialogText = mod.Helper.Translation.Get("call.scam");

            if (mod.Config.PlaySound)
                Game1.playSound("phone");

            var dlg = new Dialogue(npc, dialogText);
            Game1.DrawDialogue(dlg);
        }

        public void TriggerRandomCall()
        {
            var npcs = Utility.getAllCharacters().Where(n => n.isVillager()).ToList();
            if (npcs.Any())
                TriggerCall(npcs[Game1.random.Next(npcs.Count)], false);
        }
    }
}
}
