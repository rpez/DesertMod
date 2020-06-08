using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DesertMod.UI
{
    class DebugUI : UIState
    {
        private UIText positionText;

        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(300, 0);
            panel.Height.Set(100, 0);
            panel.Top.Set(0, 0);
            panel.Left.Set(Main.screenWidth / 2 - panel.Width.Pixels / 2, 0f);
            Append(panel);

            positionText = new UIText("0|0"); //text to show current hp or mana
            positionText.Width.Set(300, 0f);
            positionText.Height.Set(100, 0f);
            panel.Append(positionText);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.player[Main.myPlayer];
            positionText.SetText("X: " + player.position.X.ToString() + " | Y: " + player.position.Y.ToString());
            base.Update(gameTime);
        }
    }
}
