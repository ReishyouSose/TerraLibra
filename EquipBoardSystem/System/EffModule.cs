using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RUIModule;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using static TerraLibra.EquipBoardSystem.System.EquipBoardData;

namespace TerraLibra.EquipBoardSystem.System;
public enum EffModuleID : int
{
    Gem = -3,
    ScaleRight,
    ScaleLeft,
    None,
    Damage,
    Crit,
    Health,
    Mana,
    Defense,
    Move,
    Endurence,
    Dodge,
    Vampire,
    LifeRegen,
    Minion,
    Sentry,
    Count,
}
public class EffModule
{
    public readonly Asset<Texture2D> iconTex;
    public readonly EnumID<EffModuleID> type;
    public readonly int level;
    public int scale;
    public bool active;
    private Asset<Texture2D> slotTex;
    private Asset<Texture2D> activeTex;
    private string desc;
    private readonly float drawScale;
    public EffModule(int level, EnumID<EffModuleID> type)
    {
        if (level <= 0)
        {
            level = 0;
            type = 0;
        }
        this.type = type;
        this.level = level;
        scale = 1;
        if (type > 0)
        {
            int texID = EBTexs[type][level - 1];
            Main.instance.LoadItem(texID);
            iconTex = TextureAssets.Item[texID];
            drawScale = iconTex.Size().AutoScale(44);
        }
        else if (type < 0)
        {
            iconTex = extraTexs[type.ToString() + (type == EffModuleID.Gem ? "" : level)];
            drawScale = 1;
        }
    }
    public void CheckState()
    {
        slotTex = SelectSlot();
        activeTex = SelevtActive();
        desc = ChangeDesc();
    }
    private Asset<Texture2D> SelectSlot() => type.EnumValue switch
    {
        EffModuleID.Gem => TextureAssets.InventoryBack15,
        EffModuleID.None => extraTexs["None"],
        { } type when type < 0 => TextureAssets.InventoryBack11,
        _ => scale > 1 ? TextureAssets.InventoryBack14 : TextureAssets.InventoryBack,
    };
    private Asset<Texture2D> SelevtActive()
    {
        if (!active)
            return null;
        return extraTexs[scale > 1 ? "ActiveEnchant" : type < 0 ? "ActiveScale" : "ActiveNormal"];
    }
    public void DrawSelf(SpriteBatch sb, Vector2 pos, bool mouseHover)
    {
        if (mouseHover)
        {
            Main.hoverItemName = desc;
        }
        sb.Draw(slotTex.Value, pos, Color.White);
        if (active)
        {
            sb.Draw(activeTex.Value, pos, Color.White);
        }
        if (type == 0)
            return;
        if (type > 0)
        {
            sb.Draw(iconTex.Value, pos + Vector2.One * 26, null, Color.White,
                0, iconTex.Size() / 2f, drawScale, 0, 0);
        }
        else
            sb.Draw(iconTex.Value, pos, Color.White);
    }
    public string ChangeDesc()
    {
        if (type == 0)
            return "";
        LocalizedText desc = Language.GetText("Mods.EquipBoardSystem.Bonus." + type);
        int value = EBValues[type][level - 1];
        object obj;
        if (type == EffModuleID.Gem)
            return desc.Value;
        if (scale == 1)
        {
            obj = value;
        }
        else
        {
            obj = type < 0 ? (value + scale) : (value * scale + $" (*{scale})");
        }
        return desc.WithFormatArgs(obj).Value;
    }
}
