using HarmonyLib;
using StardewValley;
using StardewValley.Objects;

namespace MiniPhone.Patches
{
    internal class FurniturePatch
    {
        public static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Furniture), nameof(Furniture.checkForAction)),
                prefix: new HarmonyMethod(typeof(FurniturePatch), nameof(CheckForAction_Prefix))
            );
        }

        public static bool CheckForAction_Prefix(Furniture __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (__instance.ParentSheetIndex == 3490)
            {
                if (!justCheckingForActivity)
                {
                    Game1.showGlobalMessage("The mini phone buzzes quietly...");
                }

                __result = true;
                return false;
            }

            return true;
        }
    }
}
