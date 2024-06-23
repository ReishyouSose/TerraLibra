using System.Collections.Generic;
using TerraLibra.EquipBoardSystem.System;
using Terraria;
using Terraria.Localization;

namespace TerraLibra
{
    public static class MiscHelper
    {
        public const string LocalKey = "Mods.TerraLibra.";
        public static string GTV(string key, params object[] args) => Language.GetTextValue(LocalKey + key, args);
        public static EBItem EBI(this Item item) => item.GetGlobalItem<EBItem>();
        public static EquipBoard[] GetEBs(this Item item) => item.EBI().ebs;
        public static List<EquipBoard> GetWaitEBs(this Item item) => item.EBI().waitEbs;
    }
}
