using TerraLibra.EquipBoardSystem.System;
using Terraria.ModLoader;

namespace TerraLibra
{
    public class TerraLibra : Mod
    {
        public override void Load()
        {
            EquipBoardData.Load();
        }
    }
}
