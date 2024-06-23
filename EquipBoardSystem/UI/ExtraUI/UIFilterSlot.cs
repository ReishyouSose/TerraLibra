using TerraLibra.EquipBoardSystem.System;
using TerraLibra.EquipBoardSystem.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RUIModule.RUIElements;
using RUIModule.RUISys;
using Terraria;
using Terraria.GameContent;
using static TerraLibra.EquipBoardSystem.System.EquipBoardData;

namespace TerraLibra.EquipBoardSystem.UI.ExtraUI;

public class UIFilterSlot(int type) : UIImage(AssetLoader.Slot)
{
    private readonly Asset<Texture2D> icon = extraTexs[Select(type)];
    private readonly Asset<Texture2D> focus = extraTexs["Focus"];
    private static Player Player => Main.LocalPlayer;
    private static string Select(int type) => type switch { 0 => "Head", 1 => "Body", 2 => "Legs", _ => "Accs" };
    public bool GetCanUse()
    {
        if (id < 8) return true;
        else
        {
            int count = 8;
            if (Player.IsItemSlotUnlockedAndUsable(8)) count++;
            if (Player.IsItemSlotUnlockedAndUsable(9)) count++;
            return id < count;
        }
    }

    public override void LoadEvents()
    {
        Events.OnMouseOver += evt =>
        {
            if (GetCanUse())
                color = Color.Gold;
        };
        Events.OnMouseOut += evt => color = Color.White;
    }
    public override void DrawSelf(SpriteBatch sb)
    {
        Vector2 pos = HitBox().TopLeft();
        float opacity = GetCanUse() ? 1 : 0.5f;
        sb.Draw(Tex, pos, Color.White * opacity);
        sb.Draw(icon.Value, pos, color * opacity);
        EBSysUI ins = EBSysUI.Ins;
        if (ins.Index == id)
        {
            sb.Draw(focus.Value, pos, ins.TempEB == null ? Color.Gold : Color.Red);
        }
        pos.X += 56;
        var ebs = Player.GetModPlayer<EBPlayer>().ebs;
        for (int i = 0; i < 3; i++)
        {
            if (ebs.TryGetValue(i + id * 3, out EquipBoard eb))
            {
                sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)pos.X,
                    (int)pos.Y + i * 20, 16, 12), eb.color[1]);
            }
        }
    }
}
