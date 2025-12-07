using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace MiniPhone.UI
{
    internal class PhoneHudElement
    {
        private readonly MiniPhoneMod mod;
        private Texture2D? icon;
        private Rectangle drawRect;

        public PhoneHudElement(MiniPhoneMod mod)
        {
            this.mod = mod;
            mod.Helper.Events.Display.RenderedHud += OnRenderedHud;
            mod.Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            if (!mod.Config.ShowHudIcon || Game1.activeClickableMenu != null || !InventoryPhone.HasPhoneInInventory())
                return;

            icon ??= mod.Helper.GameContent.Load<Texture2D>("Mods/MiniPhone/Assets/PhoneIcon");
            if (icon == null) return;

            int size = mod.Config.IconSize;
            int x = Game1.viewport.Width - size - 20 + mod.Config.HudOffsetX;
            int y = 20 + mod.Config.HudOffsetY;
            drawRect = new Rectangle(x, y, size, size);

            e.SpriteBatch.Draw(icon, drawRect, Color.White * 0.9f);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!mod.Config.ShowHudIcon || !InventoryPhone.HasPhoneInInventory()) return;
            if (e.Button.IsActionButton() && drawRect.Contains(e.Cursor.ScreenPixels))
            {
                mod.Helper.Input.Suppress(e.Button);
                mod.Calls.TriggerRandomCall();
            }
        }
    }
}