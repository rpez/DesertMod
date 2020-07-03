using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using DesertMod.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace DesertMod
{
	public class DesertMod : Mod
 	{
        public static DesertMod instance;

        internal UserInterface debugInterface;
        internal DebugUI debugUI;

        public override void Load()
        {
            base.Load();

            instance = this;

            if (!Main.dedServ)
            {
                debugInterface = new UserInterface();

                debugUI = new DebugUI();
                debugUI.Activate();
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> filterRef = new Ref<Effect>(GetEffect("Effects/GlyphBurningGlow"));
                Filters.Scene["GlyphBurningGlow"] = new Filter(new ScreenShaderData(filterRef, "GlyphBurningGlow"), EffectPriority.VeryHigh);
                Filters.Scene["GlyphBurningGlow"].Load();

                Ref<Effect> filterRef2 = new Ref<Effect>(GetEffect("Effects/GlyphBurningDistort"));
                Filters.Scene["GlyphBurningDistort"] = new Filter(new ScreenShaderData(filterRef2, "GlyphBurningDistort"), EffectPriority.VeryHigh);
                Filters.Scene["GlyphBurningDistort"].Load();
            }
        }

        public override void Unload()
        {
            debugUI = null;
            base.Unload();
        }

        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (debugInterface?.CurrentState != null)
            {
                debugInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "DesertMod: DebugInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && debugInterface?.CurrentState != null)
                        {
                            debugInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        internal void ShowDebugUI()
        {
            debugInterface?.SetState(debugUI);
        }

        internal void HideDebugUI()
        {
            debugInterface?.SetState(null);
        }
    }
}