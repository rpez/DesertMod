using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria;

namespace DesertMod.UI
{
    class DebugUI : UIState
    {
        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(300, 0);
            panel.Height.Set(300, 0);
            Append(panel);

            UIText text = new UIText(Main.player[1].position.X.ToString());
            panel.Append(text);
            text = new UIText(Main.player[1].position.Y.ToString());
            panel.Append(text);
        }
    }
}
