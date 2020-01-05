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
            cbdAcBd = new C_BdAreaCode(C_RT.acs);
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

        private void GRSocketHandler_addWell(RES_STATE state)
        {
            GRSocketHandler.addWell -= GRSocketHandler_addWell;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.messageQueueBd.Enqueue("添加机井信息成功");
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("添加机井信息失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_getWells(RES_STATE state, List<C_Well> wells)
        {
            GRSocketHandler.getWells -= GRSocketHandler_getWells;
            switch (state)
            {
                case RES_STATE.OK:
                    curWellsBd = wells;
                    wndMainVM.messageQueueBd.Enqueue("获取机井信息成功");
                    break;
                case RES_STATE.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("获取机井信息失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_delWell(RES_STATE state)
        {
            GRSocketHandler.delWell -= GRSocketHandler_delWell;
            switch (state)
            {
                case RES_STATE.OK:
                    refreshCmd(strSearchKeywordBd);
                    wndMainVM.messageQueueBd.Enqueue("删除机井信息成功");
                    break;                            
                case RES_STATE.FAILED:                
                    wndMainVM.messageQueueBd.Enqueue("删除机井信息失败");
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
        public C_WellParas wpBd { get; set; } = new C_WellParas();

        public C_BdAreaCode cbdAcBd { get; set; } = new C_BdAreaCode();
        public C_BdAreaCode ebdAcBd { get; set; } = new C_BdAreaCode();
        public C_Well cwBd { get; set; } = new C_Well();
        public C_Well ewBd { get; set; } = new C_Well();

        //search
        public string strSearchResBd { get; set; }
        public string strSearchKeywordBd { get; set; }
        public List<C_Well> curWellsBd { get; set; } = new List<C_Well>();
        public C_Well curWellIndexBd { get; set; } = new C_Well();

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

        //manual add
        public void createWellCmd()
        {
            List<C_Well> wells = new List<C_Well>();
            cwBd.TsOrSt = cbdAcBd.L4Index.Name;
            cwBd.Village = cbdAcBd.L5Index.Name;
            cwBd.Loc = wpBd.LocIndex.Value;
            cwBd.TubeMat = wpBd.TubeMatIndex.Value;
            cwBd.UnitCat = wpBd.UnitCatIndex.Value;
            cwBd.PumpMode = wpBd.PumpModelIndex.Value;
            cwBd.Usefor = wpBd.UseForIndex.Value;
            if (CheckCreateWell(cwBd))
            {
                GRSocketHandler.addWell += GRSocketHandler_addWell;
                wells.Add(cwBd);
                GRSocketAPI.AddWell(wells);
            }
        }

        //search
        public void refreshCmd(string keywords)
        {
            GRSocketHandler.getWells += GRSocketHandler_getWells;
            GRSocketAPI.GetWells(keywords);
        }

        public bool CanedtWellCmd => (curWellIndexBd != null);
        public void edtWellCmd()
        {

        }

        public bool CandelWellCmd => (curWellIndexBd != null);
        public void delWellCmd()
        {
            GRSocketHandler.delWell += GRSocketHandler_delWell;
            GRSocketAPI.DelWell(curWellIndexBd.Id);
        }

        #endregion Actions

        public bool isWaitingForRefreshParas { get; set; } = false;
        public bool CheckCreateWell(C_Well w)
        {
            if (w.TsOrSt == "")
            {
                wndMainVM.messageQueueBd.Enqueue("请选择街道乡镇");
                return false;
            }
            if (w.Village == "")
            {
                wndMainVM.messageQueueBd.Enqueue("请选择所属村");
                return false;
            }
            if (w.UnitCat == "" || w.UnitCat == null)
            {
                if (wpBd.UnitCat.Count == 0)
                {
                    wndMainVM.messageQueueBd.Enqueue("请先在参数设置中添加权属单位");
                    return false;
                }
                wndMainVM.messageQueueBd.Enqueue("请选择权属单位");
                return false;
            }
            if (w.Loc == "" || w.Loc == null)
            {
                if (wpBd.Loc.Count == 0)
                {
                    wndMainVM.messageQueueBd.Enqueue("请先在参数设置中添加位置");
                    return false;
                }
                wndMainVM.messageQueueBd.Enqueue("请选择位置");
                return false;
            }
            if (w.TubeMat == "" || w.TubeMat == null)
            {
                if (wpBd.TubeMat.Count == 0)
                {
                    wndMainVM.messageQueueBd.Enqueue("请先在参数设置中添加管材");
                    return false;
                }
                wndMainVM.messageQueueBd.Enqueue("请选择管材");
                return false;
            }
            if (w.UnitCat == "" || w.UnitCat == null)
            {
                if (wpBd.UnitCat.Count == 0)
                {
                    wndMainVM.messageQueueBd.Enqueue("请先在参数设置中添加水泵型号");
                    return false;
                }
                wndMainVM.messageQueueBd.Enqueue("请选择水泵型号");
                return false;
            }
            if (w.Lng == null || w.Lng == "")
            {
                wndMainVM.messageQueueBd.Enqueue("经度不能为空");
                return false;
            }

            switch (C_Str.IsLng(w.Lng))
            {
                case -1:
                    wndMainVM.messageQueueBd.Enqueue("经度格式错误，请确认单位");
                    return false;
                case 0:
                    return false;
                case 1:
                    break;
                case 2:
                    wndMainVM.messageQueueBd.Enqueue("经度精度不足");
                    return false;
                case 3:
                    wndMainVM.messageQueueBd.Enqueue("经度超出宝坻区范围");
                    return false;
                default:
                    break;
            }


            if (w.Lat == null || w.Lat == "")
            {
                wndMainVM.messageQueueBd.Enqueue("纬度不能为空");
                return false;
            }
            switch (C_Str.IsLat(w.Lat))
            {
                case -1:
                    wndMainVM.messageQueueBd.Enqueue("纬度格式错误，请确认单位");
                    return false;
                case 0:
                    return false;
                case 1:
                    break;
                case 2:
                    wndMainVM.messageQueueBd.Enqueue("纬度精度不足");
                    return false;
                case 3:
                    wndMainVM.messageQueueBd.Enqueue("纬度超出宝坻区范围");
                    return false;
                default:
                    break;
            }
            if (w.DigTime.Year < 1950 || w.DigTime > DateTime.Now)
            {
                wndMainVM.messageQueueBd.Enqueue("请选择正确时间");
                return false;
            }


            if (w.WellDepth.ToString() == "" || w.WellDepth == 0)
            {
                wndMainVM.messageQueueBd.Enqueue("井深格式错误或为空");
                return false;
            }

            if (w.TubeIr.ToString() == "" || w.TubeIr == 0)
            {
                wndMainVM.messageQueueBd.Enqueue("井管内径格式错误或为空");
                return false;
            }

            if (w.PumpPower.ToString() == "" || w.PumpPower == 0)
            {
                wndMainVM.messageQueueBd.Enqueue("水泵动力格式错误或为空");
                return false;
            }
            return true;
        }
    }
}
