using HarmonyLib;
using StardewValley.Objects;

namespace MiniPhone.Harmony
{
    [HarmonyPatch(typeof(Furniture), nameof(Furniture.checkForAction))]
    internal static class FurniturePatch
    {
        public static void Postfix(Furniture __instance, bool justCheckingForActivity, ref bool __result)
        {
            if (__instance.ParentSheetIndex.Value == 3490)
            {
                __result = true;
                if (!justCheckingForActivity)
                    MiniPhoneMod.Instance.Calls.TriggerRandomCall();
            }
        }
    }
}