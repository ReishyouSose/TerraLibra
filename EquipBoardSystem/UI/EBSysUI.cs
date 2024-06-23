using Microsoft.Xna.Framework;
using RUIModule.RUIElements;
using System;
using System.Collections.Generic;
using TerraLibra.EquipBoardSystem.System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace TerraLibra.EquipBoardSystem.UI;

public class EBSysUI : ContainerElement
{
    internal static EBSysUI Ins;
    public EBSysUI() => Ins = this;
    private UIBottom ebView, tempView, statView, filterView, itdView;
    private static Player Player => Main.LocalPlayer;
    public EquipBoard TempEB { get; private set; }
    public Item selectItem;
    public bool needRemoveTemp;
    public override void OnInitialization()
    {
        base.OnInitialization();

        TempEB = null;

        UIPanel bg = new(140 + 312 * 3, 650, Color.Gray, 0.5f);
        bg.SetCenter(150, 0, 0.5f, 0.5f);
        bg.Info.SetMargin(20);
        bg.canDrag = true;
        bg.Info.IsSensitive = true;
        Register(bg);

        itdView = new(0, 0, 1, 1);
        itdView.Info.IsVisible = false;
        bg.Register(itdView);

        filterView = new(72, 0, 0, 1);
        filterView.SetPos(-72, 0, 1);
        filterView.DrawRec[0] = Color.White;
        bg.Register(filterView);

        ebView = new(-82, 194, 1);
        ebView.SetPos(0, 0);
        bg.Register(ebView);

        tempView = new(-82, 200, 1);
        tempView.SetPos(0, 200);
        bg.Register(tempView);

        statView = new(-82, 200, 1);
        statView.SetPos(0, 400);
        bg.Register(statView);
    }
    public override void Update(GameTime gt)
    {
        base.Update(gt);
        if (needRemoveTemp)
        {
            tempView.RemoveAll();
            statView.RemoveAll();
            Info.IsVisible = false;
            TempEB = null;
            needRemoveTemp = false;
        }
    }
    public override void OnSaveAndQuit()
    {
        tempView.RemoveAll();
        statView.RemoveAll();
        Info.IsVisible = false;
        TempEB = null;
        needRemoveTemp = false;
    }
    public void ReCalculateNowStatistics()
    {
        if (TempEB != null)
            return;
        Dictionary<int, int> stats = [];
        tempView.RemoveAll();

        UIText now = new(Language.GetTextValue("Mods.EquipBoardSystem.Info.Now"));
        now.SetPos(0, 0);
        tempView.Register(now);

        UIImage line = new(TextureAssets.MagicPixel.Value);
        line.SetSize(-10, 2, 1);
        line.SetPos(0, 28);
        tempView.Register(line);

        var ebs = selectItem.GetGlobalItem<EBItem>().ebs;

        foreach (EquipBoard eb in ebs)
        {
            for (int i = 0; i < 4; i++)
            {
                TryAddStat(stats, eb.up[i]);
                TryAddStat(stats, eb.down[i]);
            }
        }

        int x = 0, y = 1;
        for (EnumID<EffModuleID> type = 1; type.EnumValue < EffModuleID.Count; type++)
        {
            if (stats.TryGetValue(type, out int value))
            {
                Color color = Color.White;
                if (type == EffModuleID.Dodge)
                {
                    int old = value;
                    value = Math.Min(value, EBPlayer.maxDodge);
                    if (value < old)
                        color = Color.Red;
                }
                UIText desc = new(Desc(type.EnumValue, value.ToString()), color);
                desc.SetPos(x, y++ * 30 + 8);
                tempView.Register(desc);
                if (y == 6)
                {
                    y = 1;
                    x += 322;
                }
            }
        }
    }
    public void ReCalculateAllStatistics()
    {
        Dictionary<int, int> stats = [];
        statView.RemoveAll();

        UIText all = new(Language.GetTextValue("Mods.EquipBoardSystem.Info.All"));
        all.SetSize(all.TextSize);
        all.SetPos(0, 0);
        statView.Register(all);

        UIText itd = new(Language.GetTextValue("Mods.EquipBoardSystem.Info.ViewItd"));
        itd.SetSize(itd.TextSize);
        itd.SetPos(all.Width + 20, 0);
        itd.Events.OnMouseOver += evt => itd.color = Color.Gold;
        itd.Events.OnMouseOut += evt => itd.color = Color.White;
        itd.Events.OnLeftDown += evt => SeeIntroduction();
        statView.Register(itd);

        UIImage line = new(TextureAssets.MagicPixel.Value);
        line.SetSize(-10, 2, 1);
        line.SetPos(0, 28);
        statView.Register(line);

        foreach (EquipBoard eb in GetEBs())
        {
            for (int i = 0; i < 4; i++)
            {
                TryAddStat(stats, eb.up[i]);
                TryAddStat(stats, eb.down[i]);
            }
        }

        int x = 0, y = 0;
        for (EnumID<EffModuleID> type = 1; type.EnumValue < EffModuleID.Count; type++)
        {
            stats.TryGetValue(type, out int value);
            Color color = Color.White;
            if (type == EffModuleID.Dodge)
            {
                int old = value;
                value = Math.Min(value, EBPlayer.maxDodge);
                if (value < old)
                    color = Color.Red;
            }
            UIText desc = new(Desc(type.EnumValue, value.ToString()), color);
            desc.SetPos(x, ++y * 38);
            statView.Register(desc);
            if (y == 5)
            {
                y = 0;
                x += 322;
            }
        }
    }
    private static void TryAddStat(Dictionary<int, int> stats, EffModule em)
    {
        if (!em.active)
            return;
        var values = EquipBoardData.EBValues;
        int type = em.type;
        int level = em.level;
        int scale = em.scale;
        if (type <= 0)
            return;
        if (stats.ContainsKey(type))
            stats[type] += values[type][level - 1] * scale;
        else
            stats[type] = values[type][level - 1] * scale;
    }
    private static string Desc(EffModuleID type, string value, bool addIcon = true)
    {
        LocalizedText desc = Language.GetText("Mods.EquipBoardSystem.Bonus." + type).WithFormatArgs(value);
        if (addIcon)
        {
            return "[i:" + type switch
            {
                EffModuleID.Damage => ItemID.Zenith,
                EffModuleID.Crit => ItemID.EyeoftheGolem,
                EffModuleID.Health => ItemID.LifeCrystal,
                EffModuleID.Mana => ItemID.ManaCrystal,
                EffModuleID.Defense => ItemID.CobaltShield,
                EffModuleID.Move => ItemID.HermesBoots,
                EffModuleID.Endurence => ItemID.WormScarf,
                EffModuleID.Dodge => ItemID.MasterNinjaGear,
                EffModuleID.Vampire => ItemID.VampireKnives,
                EffModuleID.LifeRegen => ItemID.CharmofMyths,
                EffModuleID.Minion => ItemID.StardustDragonStaff,
                EffModuleID.Sentry => ItemID.RainbowCrystalStaff,
                _ => 0
            } + "]" + desc.Value;
        }
        return desc.Value;
    }
    private void SeeIntroduction()
    {
        if (TempEB == null)
        {
            ebView.Info.IsVisible = !ebView.IsVisible;
            tempView.Info.IsVisible = !tempView.IsVisible;
            statView.Info.IsVisible = !statView.IsVisible;
            filterView.Info.IsVisible = !filterView.IsVisible;
            itdView.Info.IsVisible = !itdView.IsVisible;
            if (itdView.IsVisible)
            {
                itdView.RemoveAll();

                UIText back = new(Language.GetTextValue("Mods.EquipBoardSystem.Info.Back"));
                back.SetSize(back.TextSize);
                back.SetPos(0, 0);
                back.Events.OnLeftDown += evt => SeeIntroduction();
                back.Events.OnMouseOver += evt => back.color = Color.Gold;
                back.Events.OnMouseOut += evt => back.color = Color.White;
                itdView.Register(back);

                UIText itd = new(Language.GetTextValue("Mods.EquipBoardSystem.Introduction"));
                itd.SetPos(0, 60);
                itdView.Register(itd);
            }
        }
    }
    private static IEnumerable<EquipBoard> GetEBs()
    {
        foreach (Item item in Player.armor[..10])
        {
            if (item.IsAir)
                continue;
            foreach (EquipBoard eb in item.GetGlobalItem<EBItem>().ebs)
            {
                if (eb == null)
                    continue;
                yield return eb;
            }
        }
    }
}
