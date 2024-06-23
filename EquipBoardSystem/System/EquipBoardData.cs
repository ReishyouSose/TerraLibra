using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLibra.EquipBoardSystem.System
{
    internal static class EquipBoardData
    {
        /// <summary>
        /// 装备盘模块对应物品贴图id
        /// </summary>
        internal static Dictionary<int, int[]> EBTexs;
        internal static Dictionary<int, int[]> EBValues;
        internal static Dictionary<string, Asset<Texture2D>> extraTexs;
        internal static Dictionary<int, Action<Player, int, int>> moduleBonus;
        private readonly static int[] valueFlat = [10, 25, 50];
        private readonly static float[] valueScale = [0.1f, 0.25f, 0.5f];
        internal static void Load()
        {
            EBTexs = [];
            moduleBonus = [];
            EBValues = [];
            static void Add(EffModuleID id, int level_1, int level_2, int level_3,
                int value_1, int value_2, int value_3, Action<Player, int, int> bonus)
            {
                int type = (int)id;
                int[] level = [level_1, level_2, level_3];
                int[] value = [value_1, value_2, value_3];
                EBValues.Add(type, value);
                if (id <= 0)
                    return;
                EBTexs.Add(type, level);
                moduleBonus.Add(type, bonus);
            }
            Add(EffModuleID.ScaleLeft, 0, 0, 0, 1, 2, 3, null);
            Add(EffModuleID.ScaleRight, 0, 0, 0, 1, 2, 3, null);
            Add(EffModuleID.None, 0, 0, 0, 0, 0, 0, null);
            DamageClass generic = DamageClass.Generic;
            Add(EffModuleID.Damage, ItemID.NightsEdge, ItemID.TerraBlade, ItemID.Zenith,
               1, 2, 3, (p, l, s) => p.GetDamage(generic) += valueScale[l - 1] * s);
            Add(EffModuleID.Crit, ItemID.RagePotion, ItemID.DestroyerEmblem, ItemID.EyeoftheGolem,
                2, 4, 6, (p, l, s) => p.GetCritChance(generic) += l * 2 * s);
            Add(EffModuleID.Health, ItemID.LifeCrystal, ItemID.LifeFruit, ItemID.AegisCrystal,
               50, 100, 150, (p, l, s) => p.statLifeMax2 += l * 50 * s);
            Add(EffModuleID.Mana, ItemID.ManaCrystal, ItemID.CrystalBall, ItemID.ArcaneCrystal,
               40, 80, 120, (p, l, s) => p.statManaMax2 += l * 40 * s);
            Add(EffModuleID.Defense, ItemID.CobaltShield, ItemID.BouncingShield, ItemID.PaladinsShield,
               10, 20, 30, (p, l, s) => p.statDefense += l * 10 * s);
            Add(EffModuleID.Move, ItemID.HermesBoots, ItemID.LightningBoots, ItemID.TerrasparkBoots,
               5, 10, 15, (p, l, s) => p.moveSpeed += l * 0.05f * s);
            Add(EffModuleID.Endurence, ItemID.EndurancePotion, ItemID.WormScarf, ItemID.FrozenTurtleShell,
               2, 5, 8, (p, l, s) => p.endurance += (l * 0.03f - 0.01f) * s);
            Add(EffModuleID.Dodge, ItemID.BrainOfConfusion, ItemID.MasterNinjaGear, ItemID.HallowedPlateMail,
               1, 3, 5, (p, l, s) => p.GetModPlayer<EBPlayer>().AddDodge((l * 2 - 1) * s));
            Add(EffModuleID.Vampire, ItemID.BatBat, ItemID.SoulDrain, ItemID.VampireKnives,
              1, 2, 3, (p, l, s) => p.GetModPlayer<EBPlayer>().AddVampire(l * s));
            Add(EffModuleID.LifeRegen, ItemID.Campfire, ItemID.HeartLantern, ItemID.CharmofMyths,
                1, 3, 5, (p, l, s) => p.lifeRegen += (l * 2 - 1) * s);
            Add(EffModuleID.Minion, ItemID.SlimeStaff, ItemID.OpticStaff, ItemID.StardustDragonStaff,
                1, 2, 3, (p, l, s) => p.maxMinions += l * s);
            Add(EffModuleID.Sentry, ItemID.HoundiusShootius, ItemID.StaffoftheFrostHydra, ItemID.RainbowCrystalStaff,
                1, 2, 3, (p, l, s) => p.maxTurrets += l * s);
        }
        public static void AssetLoader_ExtraLoad(Dictionary<string, Texture2D> extraTex)
        {
            string path = "EquipBoardSystem/Assets/";
            Asset<Texture2D> T2D(string name) => ModContent.Request<Texture2D>(path + name);
            void ExTexs(params string[] names)
            {
                extraTexs = [];
                foreach (string name in names)
                    extraTexs[name] = T2D(name);
            }
            ExTexs("None", "ScaleLeft1", "ScaleLeft2", "ScaleLeft3", "ScaleRight1", "ScaleRight2", "ScaleRight3",
                "ActiveNormal", "ActiveScale", "ActiveEnchant", "Border", "Border_New", "Back",
                "ToLeft", "ToRight", "Accept", "Reject", "Focus", "Accs", "Legs", "Body", "Head");
        }
    }
}
