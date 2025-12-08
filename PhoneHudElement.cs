using Microsoft.Xna.Framework; 
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;



namespace MiniPhone.UI

{

    internal class PhoneHudElement

    {

        private Texture2D? icon;

        private Rectangle drawRect;

        private bool iconLogged = false;



        public PhoneHudElement()

        {

            var mod = MiniPhoneMod.Instance;

            mod.Helper.Events.Display.RenderedHud += OnRenderedHud;

            mod.Helper.Events.Input.ButtonPressed += OnButtonPressed;

        }



        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)

        {

            var mod = MiniPhoneMod.Instance;

            if (!mod.Config.ShowHudIcon || Game1.activeClickableMenu != null) return;



            icon ??= mod.Helper.ModContent.Load<Texture2D>("Mods/MiniPhone/Assets/Phone16.png");

            if (icon == null) return;



            if (!iconLogged)

            {

                mod.Monitor.Log($"[MiniPhone] Icon loaded: {icon.Width}x{icon.Height}px", LogLevel.Info);

                iconLogged = true;

            }



            int size = mod.Config.IconSize;

            int x = Game1.uiViewport.Width - size - 20 + mod.Config.HudOffsetX;

            int y = 20 + mod.Config.HudOffsetY;



            drawRect = new Rectangle(x, y, size, size);

            e.SpriteBatch.Draw(icon, drawRect, Color.White * 0.9f);

        }



        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)

        {

            var mod = MiniPhoneMod.Instance;

            if (!mod.Config.ShowHudIcon) return;



            if ((e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA) &&

                drawRect.Contains(e.Cursor.ScreenPixels))

            {

                mod.Helper.Input.Suppress(e.Button);

                MiniPhoneMod.Instance.Calls.ShowCallMenu();

            }

        }

    }

}




