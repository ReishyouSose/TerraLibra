using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel.DataAnnotations;
using TerraLibra.EquipBoardSystem.UI;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLibra.EquipBoardSystem.System
{
    public class EBPlayer : ModPlayer
    {
        private int dodge;
        private int vampire;
        private int vampireCD;
        private int vampireBreak;
        internal static int maxDodge;
        internal static int vampireLimit;
        /// <summary>
        /// 元件类型的离散度，越低越倾向于物品本身的类型
        /// </summary>
        [Range(0, 1)]
        public float discrete;

        /// <summary>
        /// 优先开出已装备的物品的权重
        /// </summary>
        [Range(0, 1)]
        public float equipped;
        internal static ModKeybind check;
        public override void Load()
        {
            check = KeybindLoader.RegisterKeybind(Mod, "Check Equip", Keys.K);
        }
        public override void ResetEffects()
        {
            dodge = 0;
            vampire = 0;
            if (vampireCD > 0)
                vampireCD--;
            if (vampireBreak > 0)
                vampireBreak--;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            EBSysUI ui = EBSysUI.Ins;
            if (check.JustPressed)
            {
                if (ui.IsVisible)
                {
                    if (ui.TempEB == null)
                        ui.Info.IsVisible = false;
                }
                else
                {
                    ui.ReCalculateNowStatistics();
                    ui.ReCalculateAllStatistics();
                }
            }
        }
        public void AddDodge(int dodgeAdd) => dodge += dodgeAdd;
        public void AddVampire(int vampireAdd) => vampire += vampireAdd;
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (Main.rand.NextBool(Math.Min(maxDodge, dodge), 100))
            {
                Player.AddImmuneTime(info.CooldownCounter, Player.longInvince ? 80 : 40);
                return true;
            }
            return false;
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithItem(item, target, hit, damageDone);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.townNPC && target.type == NPCID.TargetDummy)
                return;
            int life = vampire;
            if (vampireLimit > 0)
            {
                int limit = vampireLimit * 60;
                life = (int)(vampire * Utils.GetLerpValue(limit, 0, vampireCD, true));
                vampireCD += vampire;
                if (vampireCD > limit)
                {
                    vampireBreak += vampire;
                    if (vampireBreak >= limit)
                    {
                        life = vampire;
                        vampireBreak = 0;
                    }
                    vampireCD = limit;
                }
            }
            if (life > 0)
            {
                Player.statLife += life;
                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Vector2.Zero,
                    ProjectileID.VampireHeal, 0, 0, Player.whoAmI, Player.whoAmI, life);
            }
        }
    }
}
