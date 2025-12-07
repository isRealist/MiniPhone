using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Netcode;

namespace MiniPhone
{
    public class MiniPhoneMod : Mod
    {
        public static int PhoneID = 3490;
        public static Texture2D? PhoneTexture;
        public static Rectangle PhoneRect = new(1830, 80, 64, 64);

        private Harmony? harmony;
        private static IMonitor? Monitor;
        private static DateTime lastRandomCheck = DateTime.MinValue;

        public override void Entry(IModHelper helper)
        {
            MiniPhoneMod.Monitor = base.Monitor;

            PhoneTexture = helper.ModContent.Load<Texture2D>("assets/phone.png");

            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(Furniture), nameof(Furniture.checkForAction)),
                postfix: new HarmonyMethod(typeof(MiniPhoneMod), nameof(CheckForActionPostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Phone), nameof(Phone.IsPlayerNearPhone)),
                postfix: new HarmonyMethod(typeof(MiniPhoneMod), nameof(IsNearPhonePostfix))
            );

            var phoneUpdate = AccessTools.Method(typeof(Game1), "updatePhoneRing");
            if (phoneUpdate != null)
            {
                harmony.Patch(
                    phoneUpdate,
                    postfix: new HarmonyMethod(typeof(MiniPhoneMod), nameof(BoostRingChancePostfix))
                );
            }

            MiniPhoneMod.Monitor.Log("Mini Phone v2.1 loaded", LogLevel.Info);
        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (PhoneTexture == null || Game1.activeClickableMenu != null)
                return;

            Game1.spriteBatch.Draw(PhoneTexture, PhoneRect, Color.White);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (PhoneTexture == null)
                return;

            if (e.Button.IsActionButton() &&
                PhoneRect.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                OpenCustomPhoneMenu();
            }
        }

        private static void OpenCustomPhoneMenu()
        {
            var npcs = Utility.getAllVillagers();
            if (npcs.Any())
            {
                var npc = npcs[Game1.random.Next(npcs.Count)];
                string callId = $"mod.{npc.Name.ToLower()}";
                PlayCustomRing(callId);
            }
        }

        public static void CheckForActionPostfix(Furniture __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (__instance.ParentSheetIndex == PhoneID)
            {
                if (!justCheckingForActivity)
                    OpenCustomPhoneMenu();

                __result = true;
            }
        }

        public static void IsNearPhonePostfix(ref bool __result)
        {
            if (__result)
                return;

            __result = Game1.currentLocation.furniture.Any(f =>
                f.ParentSheetIndex == PhoneID &&
                Utility.tileWithinRadiusOfPlayer(
                    (int)f.TileLocation.X,
                    (int)f.TileLocation.Y,
                    3,
                    Game1.player
                )
            );
        }

        public static void BoostRingChancePostfix()
        {
            if (Game1.random.NextDouble() < 0.05)
            {
                TriggerRandomModCall();
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.IsMultipleOf(120))
            {
                if ((DateTime.Now - lastRandomCheck).TotalSeconds > 1 &&
                    Game1.random.NextDouble() < 0.04)
                {
                    if (Phone.IsPlayerNearPhone(Game1.player))
                    {
                        TriggerRandomModCall();
                    }

                    lastRandomCheck = DateTime.Now;
                }
            }
        }

        private static void TriggerRandomModCall()
        {
            string[] modCalls =
            {
                "mod.scam1", "mod.pierre", "mod.vincent", "mod.abigail", "mod.sebastian",
                "mod.haley", "mod.emily", "mod.alex", "mod.sam", "mod.leah",
                "mod.elliott", "mod.sharlotte", "mod.gus", "mod.robin", "mod.demetrius",
                "mod.jodi", "mod.kent", "mod.lewis", "mod.marnie", "mod.pam", "mod.penny"
            };

            string randomCall = modCalls[Game1.random.Next(modCalls.Length)];
            PlayCustomRing(randomCall);
        }

        private static void PlayCustomRing(string callId)
        {
            try
            {
                Game1.playSound("miniphone_ring");

                var ringMethod =
                    typeof(Game1).GetMethod("ringPhone", BindingFlags.NonPublic | BindingFlags.Static) ??
                    typeof(Phone).GetMethod("Ring", BindingFlags.Public | BindingFlags.Static);

                ringMethod?.Invoke(null, new object[] { callId });
            }
            catch (Exception ex)
            {
                MiniPhoneMod.Monitor?.Log($"Ring failed: {ex.Message}", LogLevel.Warn);
            }
        }
    }
}
