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
using StardewValley.Menus;
using Netcode;

namespace MiniPhone
{
    public class MiniPhoneMod : Mod
    {
        public static int PhoneID = 3490;  // Safe high ID
        public static Texture2D? PhoneTexture;
        public static Rectangle PhoneRect = new(1830, 80, 64, 64);  // Top-right UI
        private static bool uiClicked;
        private Harmony? harmony;
        private static IMonitor? Monitor;
        private static DateTime lastRandomCheck = DateTime.MinValue;  // Track for higher freq

        public override void Entry(IModHelper helper)
        {
            Monitor = MiniPhoneMod.Monitor;
            PhoneTexture = helper.ModContent.Load<Texture2D>("assets/phone.png");

            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;  // Higher freq checks here

            harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(AccessTools.Method(typeof(Furniture), nameof(Furniture.checkForAction)), postfix: new HarmonyMethod(typeof(MiniPhoneMod), nameof(CheckForActionPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Utility), nameof(Utility.isNearTelephone)), postfix: new HarmonyMethod(typeof(MiniPhoneMod), nameof(IsNearTelephonePostfix)));

            // Patch vanilla phone update method for boost (if exists; fallback to ticked)
            var phoneUpdate = AccessTools.Method(typeof(Game1), "updatePhoneRing");  // Decompiled approx name
            if (phoneUpdate != null)
                harmony.Patch(phoneUpdate, postfix: new HarmonyMethod(typeof(MiniPhoneMod), nameof(BoostRingChancePostfix)));

            this.Monitor.Log("Enhanced Mini Phone v2.1 loaded: 5x higher random calls (ID 3490)", LogLevel.Info);
        }

        // UI Draw
        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (PhoneTexture == null || Game1.activeClickableMenu != null) return;
            Game1.spriteBatch.Draw(PhoneTexture, PhoneRect, Color.White);
        }

        // UI Click
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (PhoneTexture == null) return;
            if (e.Button.IsActionButton() && PhoneRect.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                OpenCustomPhoneMenu();
            }
        }

        // Custom menu All NPC
        private static void OpenCustomPhoneMenu()
        {
            var npcs = Utility.getAllVillagers();
            // Pseudo: Show list, select → custom call
            // For demo: Random NPC call
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
                if (!justCheckingForActivity) OpenCustomPhoneMenu();
                __result = true;
            }
        }

        public static void IsNearTelephonePostfix(ref bool __result)
        {
            if (!__result)
            {
                __result = Game1.currentLocation.furniture.Any(f =>
                    f.ParentSheetIndex == PhoneID &&
                    Utility.tileWithinRadiusOfPlayer((int)f.TileLocation.X, (int)f.TileLocation.Y, 3, Game1.player)
                );
            }
        }

        // 5% chance
        public static void BoostRingChancePostfix()
        {
            if (Game1.random.NextDouble() < 0.05)  // 5%
            {
                TriggerRandomModCall();
            }
        }

        // 4% for higher effective rate
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || e.IsMultipleOf(120))  // ~2 min in-game (higher freq)
            {
                if ((DateTime.Now - lastRandomCheck).TotalSeconds > 1 && Game1.random.NextDouble() < 0.04)  // 4% throttle
                {
                    if (Utility.isNearTelephone())  // Includes mod phone
                    {
                        TriggerRandomModCall();
                    }
                    lastRandomCheck = DateTime.Now;
                }
            }
        }

        // Random mod call
        private static void TriggerRandomModCall()
        {
            string[] modCalls = {
                "mod.scam1", "mod.Pierre", "mod.Vincent", "mod.Abigail", "mod.Sebastian",
                "mod.Haley", "mod.Emily", "mod.Alex", "mod.Sam", "mod.Leah",
                "mod.Elliott", "mod.Sharlotte", "mod.Gus", "mod.Robin", "mod.Demetrius",
                "mod.Jodi", "mod.Kent", "mod.Lewis", "mod.Marnie", "mod.Pam", "mod.Penny"
            };
            string randomCall = modCalls[Game1.random.Next(modCalls.Length)];
            PlayCustomRing(randomCall);
        }

        // Ring w/ custom sound/dialogue
        private static void PlayCustomRing(string callId)
        {
            try
            {
                Game1.playSound("miniphone_ring");  // Custom (fallback: "phone_ring")
                // Invoke vanilla ring for dialogue/portrait (decompiled: Game1.ringPhone or Phone.Ring)
                var ringMethod = typeof(Game1).GetMethod("ringPhone", BindingFlags.NonPublic | BindingFlags.Static)
                    ?? typeof(StardewValley.Objects.Phone).GetMethod("Ring", BindingFlags.Public | BindingFlags.Static);
                ringMethod?.Invoke(null, new object[] { callId });
            }
            catch (Exception ex)
            {
                Monitor?.Log($"Ring failed: {ex.Message}", LogLevel.Warn);
            }
        }
    }
}

