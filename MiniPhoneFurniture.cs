using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MiniPhone.Furniture
{
    internal static class MiniPhoneFurniture
    {
        public static void Register(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += (_, e) =>
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Furniture"))
                    e.Edit(d => d.AsDictionary<string, string>().Data["3490"] = "Mini Phone/64/64/-1/None/0/-1/null/Mini Phone/A tiny phone that rings from inventory!");
                if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
                    e.Edit(d => d.AsDictionary<string, string>().Data["Mini Phone"] = "388 20 337 5 335 1/Field/3490/false/null/Mini Phone");
            };
        }
    }
}