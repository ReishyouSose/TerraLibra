using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using static TerraLibra.MiscHelper;

namespace TerraLibra.EquipBoardSystem.System
{

    internal class UFCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => true;

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => GTV("Info.Condition");
    }

    internal class UntilFailure() : IItemDropRule
    {
        public List<IItemDropRuleChainAttempt> ChainedRules => [];
        public bool CanDrop(DropAttemptInfo info) => true;

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            Item item = EquipBoard.RandomChoose(info.player);
            //if (info.rng.NextFloat() < (info.npc.boss ? 0.05f : 0.001f))
            {
                item.GetGlobalItem<EBItem>().waitEbs.Add(new EquipBoard());
                Main.NewText(item.Name + "获得一块装备盘");
                return new() { State = ItemDropAttemptResultState.Success };
            }
            return new() { State = ItemDropAttemptResultState.FailedRandomRoll };
        }
    }
}
