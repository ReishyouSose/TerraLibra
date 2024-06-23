using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerraLibra.EquipBoardSystem.System
{
    public class EBItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public EquipBoard[] ebs = new EquipBoard[3];
        public List<EquipBoard> waitEbs = [];
        public override void SaveData(Item item, TagCompound tag)
        {
            TagCompound dict = [];
            for (int i = 0; i < 3; i++)
            {
                ebs[i].SaveData(out TagCompound infos);
                dict[i.ToString()] = infos;
            }
            tag["ebs"] = dict;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.TryGet("ebs", out TagCompound dict))
            {
                for (int i = 0; i < 3; i++)
                {
                    ebs[i] = EquipBoard.LoadData(i, dict.Get<TagCompound>(i.ToString()));
                }
            }
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            for (int i = 0; i < 3; i++)
            {
                ebs[i].Update(player);
            }
        }
    }
}
