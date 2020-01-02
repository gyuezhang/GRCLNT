using BruTile.Predefined;
using GRModel;
using GRSocket;
using GRUtil;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRCLNT
{
    public class PageEntWellViewModel : Screen
    {
        public PageEntWellViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
        }

        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler

        private void GRSocketHandler_getEntWellParas(RES_STATE state, C_WellParas wps)
        {
            GRSocketHandler.getEntWellParas -= GRSocketHandler_getEntWellParas;
            switch (state)
            {
                case RES_STATE.OK:
                    C_RT.ewp = new C_WellParas(wps.All);
                    wpBd = new C_WellParas(wps.All);
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("获取企业井参数失败");
                    break;
                default:
                    break;
            }
            isWaitingForRefreshParas = false;
        }

        private void GRSocketHandler_addEntWellPara(RES_STATE state)
        {
            GRSocketHandler.addEntWellPara -= GRSocketHandler_addEntWellPara;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.messageQueueBd.Enqueue("添加企业井参数成功");
                    GRSocketHandler.getEntWellParas += GRSocketHandler_getEntWellParas;
                    isWaitingForRefreshParas = true;
                    GRSocketAPI.GetEntWellParas();
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("添加企业井参数失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_delEntWellPara(RES_STATE state)
        {
            GRSocketHandler.delEntWellPara -= GRSocketHandler_delEntWellPara; ;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.messageQueueBd.Enqueue("删除企业井参数成功");
                    GRSocketHandler.getEntWellParas += GRSocketHandler_getEntWellParas;
                    isWaitingForRefreshParas = true;
                    GRSocketAPI.GetEntWellParas();
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("删除企业井参数失败");
                    break;
                default:
                    break;
            }
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
        public C_WellParas wpBd { get; set; } = C_RT.ewp;

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
            GRSocketHandler.addEntWellPara += GRSocketHandler_addEntWellPara;
            GRSocketAPI.AddEntWellPara(new C_WellPara(E_WellParaType.Loc, strPsLocBd));
        }

        public bool CanpsLocDelCmd => (wpBd.LocIndex != null && wpBd.LocIndex.Value != "" && wpBd.LocIndex.Value != null && !isWaitingForRefreshParas);
        public void psLocDelCmd()
        {
            GRSocketHandler.delEntWellPara += GRSocketHandler_delEntWellPara; ;
            GRSocketAPI.DelEntWellPara(wpBd.LocIndex);
        }

        public bool CanpsUnitCatAddCmd => (strPsUnitCatBd != null && !isWaitingForRefreshParas);
        public void psUnitCatAddCmd()
        {
            GRSocketHandler.addEntWellPara += GRSocketHandler_addEntWellPara;
            GRSocketAPI.AddEntWellPara(new C_WellPara(E_WellParaType.UnitCat, strPsUnitCatBd));
        }
        public bool CanppsUnitCatDelCmd => (wpBd.UnitCatIndex != null && wpBd.UnitCatIndex.Value != "" && wpBd.UnitCatIndex.Value != null && !isWaitingForRefreshParas);
        public void psUnitCatDelCmd()
        {
            GRSocketHandler.delEntWellPara += GRSocketHandler_delEntWellPara; ;
            GRSocketAPI.DelEntWellPara(wpBd.UnitCatIndex);
        }

        public bool CanpsTubeMatAddCmd => (strPsTubeMatBd != null && !isWaitingForRefreshParas);
        public void psTubeMatAddCmd()
        {
            GRSocketHandler.addEntWellPara += GRSocketHandler_addEntWellPara;
            GRSocketAPI.AddEntWellPara(new C_WellPara(E_WellParaType.TubeMat, strPsTubeMatBd));
        }
        public bool CanpsTubeMatDelCmd => (wpBd.TubeMatIndex != null && wpBd.TubeMatIndex.Value != "" && wpBd.TubeMatIndex.Value != null && !isWaitingForRefreshParas);
        public void psTubeMatDelCmd()
        {
            GRSocketHandler.delEntWellPara += GRSocketHandler_delEntWellPara; ;
            GRSocketAPI.DelEntWellPara(wpBd.TubeMatIndex);
        }

        public bool CanpsPumpModelAddCmd => (strPsPumpModelBd != null && !isWaitingForRefreshParas);
        public void psPumpModelAddCmd()
        {
            GRSocketHandler.addEntWellPara += GRSocketHandler_addEntWellPara;
            GRSocketAPI.AddEntWellPara(new C_WellPara(E_WellParaType.PumpModel, strPsPumpModelBd));
        }
        public bool CanpsPumpModelDelCmd => (wpBd.PumpModelIndex != null && wpBd.PumpModelIndex.Value != "" && wpBd.PumpModelIndex.Value != null && !isWaitingForRefreshParas);
        public void psPumpModelDelCmd()
        {
            GRSocketHandler.delEntWellPara += GRSocketHandler_delEntWellPara; ;
            GRSocketAPI.DelEntWellPara(wpBd.PumpModelIndex);
        }

        public bool CanpsUseForAddCmd => (strPsUseForBd != null && !isWaitingForRefreshParas);
        public void psUseForAddCmd()
        {
            GRSocketHandler.addEntWellPara += GRSocketHandler_addEntWellPara;
            GRSocketAPI.AddEntWellPara(new C_WellPara(E_WellParaType.UseFor, strPsUseForBd));
        }
        public bool CanpsUseForDelCmd => (wpBd.UseForIndex != null && wpBd.UseForIndex.Value != "" && wpBd.UseForIndex.Value != null && !isWaitingForRefreshParas);
        public void psUseForDelCmd()
        {
            GRSocketHandler.delEntWellPara += GRSocketHandler_delEntWellPara; ;
            GRSocketAPI.DelEntWellPara(wpBd.UseForIndex);
        }
        #endregion Actions
        public bool isWaitingForRefreshParas { get; set; } = false;

    }
}
