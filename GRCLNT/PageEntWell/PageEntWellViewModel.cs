using GRModel;
using GRSocket;
using GRUtil;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.UI.Wpf;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace GRCLNT
{
    public class PageEntWellViewModel : Screen
    {
        public PageEntWellViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
            wpBd = C_RT.wp;
            cbdAcBd = new C_BdAreaCode(C_RT.acs);
            ebdAcBd = new C_BdAreaCode(C_RT.acs);
        }

        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler

        private void GRSocketHandler_getEntWellParas(E_ResState state, C_WellParas wps)
        {
            GRSocketHandler.getEntWellParas -= GRSocketHandler_getEntWellParas;
            switch (state)
            {
                case E_ResState.OK:
                    C_RT.ewp = new C_WellParas(wps.All);
                    wpBd = new C_WellParas(wps.All);
                    break;
                case E_ResState.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("获取企业井参数失败");
                    break;
                default:
                    break;
            }
            isWaitingForRefreshParas = false;
        }

        private void GRSocketHandler_addEntWellPara(E_ResState state)
        {
            GRSocketHandler.addEntWellPara -= GRSocketHandler_addEntWellPara;
            switch (state)
            {
                case E_ResState.OK:
                    wndMainVM.messageQueueBd.Enqueue("添加企业井参数成功");
                    GRSocketHandler.getEntWellParas += GRSocketHandler_getEntWellParas;
                    isWaitingForRefreshParas = true;
                    GRSocketAPI.GetEntWellParas();
                    break;
                case E_ResState.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("添加企业井参数失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_delEntWellPara(E_ResState state)
        {
            GRSocketHandler.delEntWellPara -= GRSocketHandler_delEntWellPara; ;
            switch (state)
            {
                case E_ResState.OK:
                    wndMainVM.messageQueueBd.Enqueue("删除企业井参数成功");
                    GRSocketHandler.getEntWellParas += GRSocketHandler_getEntWellParas;
                    isWaitingForRefreshParas = true;
                    GRSocketAPI.GetEntWellParas();
                    break;
                case E_ResState.FAILED:
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
        public C_EntWell cwBd { get; set; } = new C_EntWell();
        public static C_EntWell ewBd { get; set; } = new C_EntWell();

        //search
        public string strSearchResBd { get; set; }
        public string strSearchKeywordBd { get; set; }
        public List<C_EntWell> curWellsBd { get; set; } = new List<C_EntWell>();
        public C_EntWell curWellIndexBd { get; set; } = new C_EntWell();
        //loc
        public MapControl mapBd { get; set; } = null;

        //auto add
        public string inputFilePathBd { get; set; }
        public ObservableCollection<string> autoAddLogBd { get; set; } = new ObservableCollection<string>();
        public int vInputProgBarBd { get; set; } = 0;
        public Visibility vErrLogBd { get; set; } = Visibility.Collapsed; 
        public Visibility vInputingBd { get; set; } = Visibility.Collapsed;
        public string txtReadAutoInputingBd { get; set; } = "";

        //output
        public C_WellOutput opBd { get; set; } = new C_WellOutput();

        //state
        public List<string> tsWellCntLabelBd { get; set; } = new List<string>();
        public ChartValues<ObservableValue> tsWellCntBd { get; set; } = new ChartValues<ObservableValue>();
        public SeriesCollection useForSeriesBd { get; set; } = new SeriesCollection(); 
        public SeriesCollection tubeMatSeriesBd { get; set; } = new SeriesCollection();

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
            ewBd = curWellIndexBd;
            wndMainVM.SelectPage(E_Page.Well_Edit);
        }

        public bool CandelWellCmd => (curWellIndexBd != null);
        public void delWellCmd()
        {
            GRSocketHandler.delWell += GRSocketHandler_delWell;
            GRSocketAPI.DelWell(curWellIndexBd.Id);
        }

        public void saveEdtWellCmd()
        {
            ewBd.TsOrSt = ebdAcBd.L4Index.Name;
            ewBd.Village = ebdAcBd.L5Index.Name;
            ewBd.Loc = wpBd.LocIndex.Value;
            ewBd.TubeMat = wpBd.TubeMatIndex.Value;
            ewBd.UnitCat = wpBd.UnitCatIndex.Value;
            ewBd.PumpMode = wpBd.PumpModelIndex.Value;
            ewBd.Usefor = wpBd.UseForIndex.Value;
            if (CheckCreateWell(ewBd))
            {
                GRSocketHandler.edtWell += GRSocketHandler_edtWell;
                GRSocketAPI.EdtWell(ewBd);
            }
        }

        //autoadd
        public bool CaninputOpenDlgCmd => !IsReadingFromExcel;
        public void inputOpenDlgCmd()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xlsx";
            ofd.Filter = "Excel文件|*.xlsx;*.xls";
            if (ofd.ShowDialog() == true)
            {
                inputFilePathBd = ofd.FileName;
            }
        }

        public void loadWellFromExcelCmd()
        {
            IsReadingFromExcel = true;
            vInputingBd = Visibility.Visible;
            iErrCount = 0;
            autoAddLogBd = new ObservableCollection<string>();
            vErrLogBd = Visibility.Collapsed;
            C_ExcelOper.readWell += C_ExcelOper_readWell;
            C_ExcelOper.ReadWellsFromFile(inputFilePathBd);
        }

        public void loadWellToSvrCmd()
        {
            GRSocketHandler.addWell += GRSocketHandler_addWell;
            GRSocketAPI.AddWell(autoLoadWells);
        }

        public void openTemplateCmd()
        {
            C_ExcelOper.OpenInputTemplete();
        }

        //output
        public void selectOutPutDirCmd()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            // dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            // dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                opBd.OpPath = dlg.FileName;
            }
        }

        public void StartOutPutCmd()
        {
            C_ExcelOper.OutputWell(opBd, curWellsBd);
        }


        #endregion Actions
        public bool isWaitingForRefreshParas { get; set; } = false;

    }
}
