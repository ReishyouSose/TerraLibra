using System;
using System.ComponentModel;
using TerraLibra.EquipBoardSystem.UI;
using Terraria;
using Terraria.ModLoader.Config;

namespace TerraLibra.EquipBoardSystem.System
{
    public class EBConfig : ModConfig
    {
        internal static EBConfig Ins;
        public EBConfig() => Ins = this;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public bool balanceMode;

        [Slider]
        [Range(10, 90)]
        [DefaultValue(90)]
        public int MaxDodge;

        [Slider]
        [Range(0, 10)]
        [DefaultValue(1)]
        public int VampireCD;
        public override void OnChanged()
        {
            EBPlayer.maxDodge = MaxDodge;
            EBPlayer.vampireLimit = VampireCD;
            if (Main.gameMenu)
                return;
            EBSysUI ui = EBSysUI.Ins;
            if (ui.TempEB == null)
            {
                ui.ReCalculateNowStatistics();
                ui.ReCalculateAllStatistics();
            }
        }
    }
}
