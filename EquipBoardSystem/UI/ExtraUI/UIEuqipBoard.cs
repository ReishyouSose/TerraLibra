using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RUIModule.RUIElements;
using TerraLibra.EquipBoardSystem.System;
using Terraria;
using Terraria.Localization;
using static TerraLibra.EquipBoardSystem.System.EquipBoardData;

namespace TerraLibra.EquipBoardSystem.UI.ExtraUI
{
    //按钮起点206,170,26
    //slot起点X34, Y上34，下108
    public class UIEuqipBoard(EquipBoard eb, Item item, bool newEB = false) : UIImage(extraTexs[newEB ? "Border_New" : "Border"].Value)
    {
        private readonly EquipBoard eb = eb;
        private readonly Asset<Texture2D> bg = extraTexs["Back"];
        private Item item = item;
        private int index;
        public override void OnInitialization()
        {
            if (eb != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    RegisterEffModule(eb.up[i], i * 62 + 34, 34, i, newEB);
                    RegisterEffModule(eb.down[i], i * 62 + 34, 108, i + 4, newEB);
                }
            }
            if (newEB)
            {
                UIImage reject = new(extraTexs["Reject"].Value);
                reject.SetPos(180, 170);
                reject.Events.OnLeftDoubleClick += evt => EBSysUI.Ins.needRemoveTemp = true;
                reject.hoverText = Language.GetTextValue("Mods.EquipBoardSystem.Info.Reject");
                UnHoverHidden(reject);
                Register(reject);

                UIImage accept = new(extraTexs["Accept"].Value);
                accept.SetPos(206, 170);
                accept.Events.OnLeftDoubleClick += evt =>
                {
                    int count = 0;
                    var ebs = item.GetGlobalItem<EBItem>().ebs;
                    for (int i = 0; i < 3; i++)
                    {
                        if (ebs[i] != null)
                        {
                            count++;
                        }
                        else
                            break;
                    }
                    if (count < 3)
                        index = count;
                    ebs[index] = eb.Clone(index);
                    EBSysUI.Ins.needRemoveTemp = true;
                };
                accept.hoverText = Language.GetTextValue("Mods.EquipBoardSystem.Info.Accept");
                UnHoverHidden(accept);
                Register(accept);

                UIImage toLeft = new(extraTexs["ToLeft"].Value);
                toLeft.SetPos(232, 170);
                toLeft.Events.OnLeftDown += evt =>
                {
                    float x = Info.Left.Pixel;
                    if (x == 0)
                        return;
                    index--;
                    SetPos(x - 322, 0);
                };
                toLeft.hoverText = Language.GetTextValue("Mods.EquipBoardSystem.Info.Left");
                UnHoverHidden(toLeft);
                Register(toLeft);

                UIImage toRight = new(extraTexs["ToRight"].Value);
                toRight.SetPos(258, 170);
                toRight.Events.OnLeftDown += evt =>
                {
                    float x = Info.Left.Pixel;
                    if (x == 644)
                        return;
                    index++;
                    SetPos(x + 322, 0);
                };
                toRight.hoverText = Language.GetTextValue("Mods.EquipBoardSystem.Info.Right");
                UnHoverHidden(toRight);
                Register(toRight);
            }
        }
        private static void UnHoverHidden(BaseUIElement uie)
        {
            uie.Info.IsHidden = true;
            uie.Events.OnMouseOver += evt => evt.Info.IsHidden = false;
            uie.Events.OnMouseOut += evt => evt.Info.IsHidden = true;
        }
        private void RegisterEffModule(EffModule em, int x, int y, int index, bool newEB)
        {
            UIEffModule uem = new(em);
            uem.SetPos(x, y);
            uem.Events.OnLeftDoubleClick += evt =>
            {
                int i = index;
                eb.ChangeState(i);
                EBSysUI.Ins.ReCalculateNowStatistics();
            };
            if (!newEB)
                uem.Events.OnLeftDoubleClick += evt => EBSysUI.Ins.ReCalculateAllStatistics();
            Register(uem);
        }
        public override void DrawSelf(SpriteBatch sb)
        {
            sb.Draw(bg.Value, HitBox(), eb?.color[0] ?? Color.Transparent);
            base.DrawSelf(sb);
        }
    }
}
