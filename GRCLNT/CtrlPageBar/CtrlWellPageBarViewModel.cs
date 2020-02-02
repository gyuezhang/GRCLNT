using GRModel;
using Stylet;
using System.Collections.Generic;

namespace GRCLNT
{
    public class CtrlWellPageBarViewModel : Screen
    {
        public CtrlWellPageBarViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
        }
        private WndMainViewModel wndMainVM { get; set; }

        #region Bindings

        public int allCntBd { get; set; }
        public int pIndexBd { get; set; }
        public int pLastBd { get; set; }
        public int pJumpBd { get; set; }
        public List<C_Well> curItemsBd { get; set; }
        public List<C_Well> allItemsBd { get; set; }

        #endregion Bindings

        #region Actions

        public void JumpBmd()
        {
            try
            {
                if(pJumpBd>pLastBd || pJumpBd<1)
                {
                    wndMainVM.messageQueueBd.Enqueue("不存在此页");
                    return;
                }
                Update(pJumpBd);
            }
            catch
            {

            }
        }

        public bool CanHomeCmd => pIndexBd != 1;
        public void HomeCmd()
        {
            Update();
        }

        public bool CanEndCmd => pIndexBd != pLastBd;
        public void EndCmd()
        {
            Update(pLastBd);
        }

        public bool CanPreCmd => pIndexBd > 1;
        public void PreCmd()
        {
            pIndexBd--;
            Update(pIndexBd);
        }

        public bool CanNextCmd => pIndexBd < pLastBd;
        public void NextCmd()
        {
            pIndexBd++;
            Update(pIndexBd);
        }
        
        #endregion Actions

        private int unitLen { get; set; } = 50;

        public void Update(int i=1)
        {
            pIndexBd = i;
            int count = unitLen;
            if (pIndexBd == pLastBd)
                count = allCntBd % unitLen;
            curItemsBd = allItemsBd.GetRange((pIndexBd - 1) * unitLen, count);
        }

        public void Init(List<C_Well> logs)
        {
            allItemsBd = logs;
            allCntBd = allItemsBd.Count;
            pIndexBd = 1;
            pLastBd = allCntBd / unitLen + 1;
            Update();
        }
    }
}
