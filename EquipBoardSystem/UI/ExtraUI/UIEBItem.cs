using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RUIModule.RUIElements;
using TerraLibra.EquipBoardSystem.System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace TerraLibra.EquipBoardSystem.UI.ExtraUI
{
    public class UIEBItem(int index) : UIItemSlot()
    {
        public static EBSysUI Ins => EBSysUI.Ins;
        public readonly int index = index;
        public override void LoadEvents()
        {
            Events.OnLeftDown += evt =>
            {
                if (!item.IsAir)
                {
                    Ins.ChangeView(item);
                }
            };
            Events.OnRightDown += evt =>
            {
                if (item?.IsAir != false)
                    return;
                item.GetWaitEBs().Add(new());
                Ins.NewEB(item);
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            item = Main.LocalPlayer.armor[index];
            if (item.type == ItemID.None)
                return;
            DrawRec[0] = Ins.SelectItem != item ? null : Ins.TempEB == null ? Color.Gold : Color.Red;
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            if (item.type == ItemID.None)
                return;
            var ebs = item.GetEBs();
            Texture2D tex = TextureAssets.MagicPixel.Value;
            Point pos = new(Right + 4, Top);
            for (int i = 0; i < 3; i++)
            {
                EquipBoard eb = ebs[i];
                if (eb != null)
                {
                    sb.Draw(tex, new Rectangle(pos.X, pos.Y + i * 20, 16, 12), eb.color[1]);
                }
            }
        }
    }
}
