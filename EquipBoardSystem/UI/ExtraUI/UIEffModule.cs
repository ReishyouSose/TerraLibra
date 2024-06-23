using Microsoft.Xna.Framework.Graphics;
using RUIModule.RUIElements;
using RUIModule.RUISys;
using TerraLibra.EquipBoardSystem.System;

namespace TerraLibra.EquipBoardSystem.UI.ExtraUI
{
    public class UIEffModule(EffModule em) : UIImage(AssetLoader.Slot)
    {
        public EffModule EffModule { get; private set; } = em;

        public override void DrawSelf(SpriteBatch sb) => EffModule.DrawSelf(sb, new(Left, Top), Info.IsMouseHover);
    }
}
