using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using static Terraria.GameContent.UI.ItemRarity;

namespace TerraLibra.EquipBoardSystem.System;

public class EquipBoard
{
    public readonly int level;
    public readonly Color[] color;
    public readonly EffModule[] up, down;
    public readonly int index;
    public EquipBoard()
    {
        level = 1;
        if (Main.hardMode)
            level = 2;
        if (NPC.downedPlantBoss)
            level = 3;
        if (NPC.downedMoonlord)
            level = 4;
        up = new EffModule[4];
        down = new EffModule[4];
        var rng = Main.rand;
        level = Main.rand.Next(level * level) switch
        {
            15 => 4,
            13 or 10 or 7 => 3,
            14 or 11 or 8 or 5 or 3 => 2,
            _ => 1
        };
        color = level switch
        {
            4 => [GetColor(ItemRarityID.LightRed), GetColor(ItemRarityID.Red)],
            3 => [GetColor(ItemRarityID.Orange), GetColor(ItemRarityID.Yellow)],
            2 => [GetColor(ItemRarityID.Pink), GetColor(ItemRarityID.Purple)],
            _ => [GetColor(ItemRarityID.Blue), GetColor(ItemRarityID.Cyan)]
        };
        int[] levels = RandomLevel(level, rng);
        for (int i = 0; i < 4; i++)
        {
            int level = levels[i];
            if (level == 0)
            {
                up[i] = new(0, 0);
                continue;
            }
            else
            {
                int type = RollEffModule(rng, ref level);
                up[i] = new(level, type);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            int level = levels[i + 4];
            if (level == 0)
            {
                down[i] = new(0, 0);
                continue;
            }
            else
            {
                int type = RollEffModule(rng, ref level);
                down[i] = new(level, type);
            }
        }
        for (int i = 0; i < 4; i++)
            ChangeState(i);
    }
    public EquipBoard((int level, EnumID<EffModuleID> type)[] data, int[] active, int ebLevel, int index)
    {
        this.index = index;
        up = new EffModule[4];
        down = new EffModule[4];
        for (int i = 0; i < 8; i++)
        {
            int level = data[i].level, type = data[i].type;
            if (i < 4)
                up[i] = new(level, type);
            else
                down[i - 4] = new(level, type);
        }
        level = ebLevel;
        color = level switch
        {
            4 => [GetColor(ItemRarityID.LightRed), GetColor(ItemRarityID.Red)],
            3 => [GetColor(ItemRarityID.Orange), GetColor(ItemRarityID.Yellow)],
            2 => [GetColor(ItemRarityID.Pink), GetColor(ItemRarityID.Purple)],
            _ => [GetColor(ItemRarityID.Blue), GetColor(ItemRarityID.Cyan)]
        };
        foreach (int i in active)
            ChangeState(i);
    }
    private int RollEffModule(Terraria.Utilities.UnifiedRandom rng, ref int emlevel)
    {
        if (rng.NextBool(level * level, 50))
        {
            return -rng.Next(3) - 1;
        }
        return rng.Next((int)EffModuleID.Count);
    }
    public void Update(Player player)
    {
        for (int i = 0; i < 4; i++)
        {
            TryUpdate(player, up[i]);
            TryUpdate(player, down[i]);
        }
    }
    public void ChangeState(int index)
    {
        if (index < 4)
        {
            up[index].active = true;
            down[index].active = false;
        }
        else
        {
            index -= 4;
            up[index].active = false;
            down[index].active = true;
        }
        CheckScale();
        for (int i = 0; i < 4; i++)
        {
            up[i].CheckState();
            down[i].CheckState();
        }
    }
    private static void TryUpdate(Player player, EffModule m)
    {
        if (m.active && EquipBoardData.moduleBonus.TryGetValue(m.type, out Action<Player, int, int> bonus))
        {
            bonus.Invoke(player, m.level, m.scale);
        }
    }
    public void CheckScale()
    {
        CheckScale(up);
        CheckScale(down);
    }
    private static void CheckScale(EffModule[] modules)
    {
        for (int i = 0; i < 4; i++)
            modules[i].scale = 1;
        for (int i = 0; i < 3; i++)
        {
            EffModule em = modules[i];
            if (em.active && em.type == EffModuleID.ScaleRight)
            {
                EffModule next = modules[i + 1];
                if (next.type.EnumValue is not EffModuleID.ScaleLeft or EffModuleID.None)
                    next.scale += em.level + em.scale - 1;
            }
        }
        for (int i = 3; i > 0; i--)
        {
            EffModule em = modules[i];
            if (em.active && em.type == EffModuleID.ScaleLeft)
            {
                EffModule next = modules[i - 1];
                if (next.type.EnumValue is not EffModuleID.ScaleRight or EffModuleID.None)
                    next.scale += em.level + em.scale - 1;
            }
        }
    }
    private static int[] RandomLevel(int level, Terraria.Utilities.UnifiedRandom rng)
    {
        int[] levels = new int[8];
        for (int i = 0; i < 8; i++)
        {
            bool down = rng.NextBool(level + 4);
            bool none = down && rng.NextBool(level + 4);
            levels[i] = Math.Min(none ? 0 : level - (down ? 1 : 0), 3);
        }
        return levels;
    }
    public EquipBoard Clone(int index)
    {
        (int level, EnumID<EffModuleID> type)[] data = new (int level, EnumID<EffModuleID>)[8];
        List<int> active = [];
        for (int i = 0; i < 8; i++)
        {
            EffModule em = i < 4 ? up[i] : down[i - 4];
            data[i] = (em.level, em.type);
            if (em.active)
            {
                active.Add(i);
            }
        }
        return new EquipBoard(data, [.. active], level, index);
    }
    public void SaveData(out TagCompound infos)
    {
        infos = [];
        infos["level"] = level;
        List<int> active = [];
        for (int i = 0; i < 8; i++)
        {
            TagCompound info = [];
            EffModule em = i < 4 ? up[i] : down[i - 4];
            info["level"] = em.level;
            info["type"] = em.type.IntValue;
            infos[$"{i}"] = info;
            if (em.active)
            {
                active.Add(i);
            }
        }
        if (active.Count == 0)
            active = [1, 2, 3, 4];
        infos["active"] = active.ToArray();
    }
    public static EquipBoard LoadData(int index, TagCompound infos)
    {
        (int level, EnumID<EffModuleID> type)[] data = new (int level, EnumID<EffModuleID> type)[8];
        for (int i = 0; i < 8; i++)
        {
            if (infos.TryGet(i.ToString(), out TagCompound info))
            {
                data[i] = (info.GetInt("level"), info.GetInt("type"));
            }
        }
        return new EquipBoard(data, infos.GetIntArray("active"), infos.GetInt("level"), index);
    }
    public static Item RandomChoose(Player p)
    {
        EBPlayer ebp = p.GetModPlayer<EBPlayer>();
        List<Item> choose = [];
        choose.AddRange(p.armor[..10].Where(x => !x.IsAir));
        var rng = Main.rand;
        if (rng.NextFloat() < ebp.equipped)
        {
            foreach (Item item in p.inventory)
            {
                if (!item.IsAir && item.damage > 0 && !item.accessory)
                {
                    if (item == p.HeldItem || p.ownedProjectileCounts[item.shoot] > 0)
                        choose.Add(item);
                }
            }
        }
        else
        {
            foreach (Item item in p.inventory)
            {
                if (!item.IsAir && (item.damage > 0 || item.accessory))
                {
                    choose.Add(item);
                }
            }
        }
        return choose[rng.Next(choose.Count)];
    }
}
