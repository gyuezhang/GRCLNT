using GRModel;
using GRSocket;
using GRUtil;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRCLNT
{
    public class PageWellViewModel : Screen
    {
        public PageWellViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
            wpBd = C_RT.wp;
        }
        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler

        private void GRSocketHandler_addWellPara(RES_STATE state)
        {
            GRSocketHandler.addWellPara -= GRSocketHandler_addWellPara;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.messageQueueBd.Enqueue("添加机井参数成功");
                    GRSocketHandler.getWellParas += GRSocketHandler_getWellParas;
                    isWaitingForRefreshParas = true;
                    GRSocketAPI.GetWellParas();
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("添加机井参数失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_delWellPara(RES_STATE state)
        {
            GRSocketHandler.delWellPara -= GRSocketHandler_delWellPara;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.messageQueueBd.Enqueue("删除机井参数成功");
                    GRSocketHandler.getWellParas += GRSocketHandler_getWellParas;
                    isWaitingForRefreshParas = true;
                    GRSocketAPI.GetWellParas();
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("删除机井参数失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_getWellParas(RES_STATE state, C_WellParas wps)
        {
            GRSocketHandler.getWellParas -= GRSocketHandler_getWellParas;
            switch (state)
            {
                case RES_STATE.OK:
                    C_RT.wp = new C_WellParas(wps.All);
                    wpBd = new C_WellParas(wps.All);
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("获取机井参数失败");
                    break;
                default:
                    break;
            }
            isWaitingForRefreshParas = false;
        }

        #endregion SocketHandler

        #region Bindings
        public int pageIndexBd { get; set; } = 0;

        //parasetting
        public string strPsLocBd { get; set; }
        public string strPsUnitCatBd { get; set; }
        public string strPsTubeMatBd { get; set; }
        public string strPsPumpModelBd { get; set; }
        public string strPsUseForBd { get; set; }
        public C_WellParas wpBd { get; set; } = new C_WellParas();

        public C_BdAreaCode cbdAcBd = new C_BdAreaCode(C_RT.acs);
        public C_BdAreaCode ebdAcBd = new C_BdAreaCode(C_RT.acs);

        #endregion Bindings

        #region Actions
        public void SelectPageCmd(string cmdPara)
        {
            wndMainVM.SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }

        //parasetting
        public bool CanpsLocAddCmd => (strPsLocBd != null && !isWaitingForRefreshParas);
        public void psLocAddCmd()
        {
            GRSocketHandler.addWellPara += GRSocketHandler_addWellPara;
            GRSocketAPI.AddWellPara(new C_WellPara(E_WellParaType.Loc, strPsLocBd));
        }

        public bool CanpsLocDelCmd => (wpBd.LocIndex != null && wpBd.LocIndex.Value != "" && wpBd.LocIndex.Value != null && !isWaitingForRefreshParas);
        public void psLocDelCmd()
        {
            GRSocketHandler.delWellPara += GRSocketHandler_delWellPara;
            GRSocketAPI.DelWellPara(wpBd.LocIndex);
        }

        public bool CanpsUnitCatAddCmd => (strPsUnitCatBd != null && !isWaitingForRefreshParas);
        public void psUnitCatAddCmd()
        {
            GRSocketHandler.addWellPara += GRSocketHandler_addWellPara;
            GRSocketAPI.AddWellPara(new C_WellPara(E_WellParaType.UnitCat, strPsUnitCatBd));
        }
        public bool CanppsUnitCatDelCmd => (wpBd.UnitCatIndex != null && wpBd.UnitCatIndex.Value != "" && wpBd.UnitCatIndex.Value != null && !isWaitingForRefreshParas);
        public void psUnitCatDelCmd()
        {
            GRSocketHandler.delWellPara += GRSocketHandler_delWellPara;
            GRSocketAPI.DelWellPara(wpBd.UnitCatIndex);
        }

        public bool CanpsTubeMatAddCmd => (strPsTubeMatBd != null && !isWaitingForRefreshParas);
        public void psTubeMatAddCmd()
        {
            GRSocketHandler.addWellPara += GRSocketHandler_addWellPara;
            GRSocketAPI.AddWellPara(new C_WellPara(E_WellParaType.TubeMat, strPsTubeMatBd));
        }
        public bool CanpsTubeMatDelCmd => (wpBd.TubeMatIndex != null && wpBd.TubeMatIndex.Value != "" && wpBd.TubeMatIndex.Value != null && !isWaitingForRefreshParas);
        public void psTubeMatDelCmd()
        {
            GRSocketHandler.delWellPara += GRSocketHandler_delWellPara;
            GRSocketAPI.DelWellPara(wpBd.TubeMatIndex);
        }

        public bool CanpsPumpModelAddCmd => (strPsPumpModelBd != null && !isWaitingForRefreshParas);
        public void psPumpModelAddCmd()
        {
            GRSocketHandler.addWellPara += GRSocketHandler_addWellPara;
            GRSocketAPI.AddWellPara(new C_WellPara(E_WellParaType.PumpModel, strPsPumpModelBd));
        }
        public bool CanpsPumpModelDelCmd => (wpBd.PumpModelIndex != null && wpBd.PumpModelIndex.Value != "" && wpBd.PumpModelIndex.Value != null && !isWaitingForRefreshParas);
        public void psPumpModelDelCmd()
        {
            GRSocketHandler.delWellPara += GRSocketHandler_delWellPara;
            GRSocketAPI.DelWellPara(wpBd.PumpModelIndex);
        }

        public bool CanpsUseForAddCmd => (strPsUseForBd != null && !isWaitingForRefreshParas);
        public void psUseForAddCmd()
        {
            GRSocketHandler.addWellPara += GRSocketHandler_addWellPara;
            GRSocketAPI.AddWellPara(new C_WellPara(E_WellParaType.UseFor, strPsUseForBd));
        }
        public bool CanpsUseForDelCmd => (wpBd.UseForIndex != null && wpBd.UseForIndex.Value != "" && wpBd.UseForIndex.Value != null && !isWaitingForRefreshParas);
        public void psUseForDelCmd()
        {
            GRSocketHandler.delWellPara += GRSocketHandler_delWellPara;
            GRSocketAPI.DelWellPara(wpBd.UseForIndex);
        }
        #endregion Actions

        public bool isWaitingForRefreshParas { get; set; } = false;

    }
}
