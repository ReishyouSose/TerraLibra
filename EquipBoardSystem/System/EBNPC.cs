using Terraria;
using Terraria.ModLoader;

namespace TerraLibra.EquipBoardSystem.System
{
    public class EBNPC : GlobalNPC
    {
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (npc.life <= 0)
            {

            }
        }
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new UntilFailure());
        }
    }
}
