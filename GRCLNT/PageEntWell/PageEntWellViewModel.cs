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
using Mapsui.Desktop.Shapefile;
using Mapsui.Styles.Thematics;
using MaterialDesignThemes.Wpf;

namespace GRCLNT
{
    public class PageEntWellViewModel : Screen
    {
        public PageEntWellViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
            pageBarVmBd = new CtrlEntWellPageBarViewModel(wndMainVM);
            wpBd = C_RT.ewp;
            wpForSearchBd = new C_WellParas(C_RT.ewp.All);
            wpForSearchBd.Loc.Add(new C_WellPara(E_WellParaType.Loc, "全部"));
            wpForSearchBd.UnitCat.Add(new C_WellPara(E_WellParaType.UnitCat, "全部"));
            wpForSearchBd.TubeMat.Add(new C_WellPara(E_WellParaType.TubeMat, "全部"));
            wpForSearchBd.UseFor.Add(new C_WellPara(E_WellParaType.UseFor, "全部"));
            wpForSearchBd.PumpModel.Add(new C_WellPara(E_WellParaType.PumpModel, "全部"));
            cbdAcBd = new C_BdAreaCode(C_RT.acs);
            ebdAcBd = new C_BdAreaCode(C_RT.acs);
            scAcBd = new C_BdAreaCode(C_RT.acs);
            foreach (C_AreaCode ac in scAcBd.AllL4AreaCodes)
            {
                C_AreaCode t = new C_AreaCode();
                t.Id = -1; t.Level = 5; t.PCode = ac.Code; t.Code = -1; t.Name = "全街道（乡镇）";
                scAcBd.AllL5AreaCodes.Add(t);
            }
            C_AreaCode acTmp4 = new C_AreaCode();
            acTmp4.Id = 0; acTmp4.Level = 4; acTmp4.PCode = 0; acTmp4.Code = 0; acTmp4.Name = "全区";
            C_AreaCode acTmp5 = new C_AreaCode();
            acTmp5.Id = 0; acTmp5.Level = 5; acTmp5.PCode = 0; acTmp5.Code = 0; acTmp5.Name = "全街道（乡镇）";

            scAcBd.AllL4AreaCodes.Add(acTmp4);
            scAcBd.AllL5AreaCodes.Add(acTmp5);
            ClearSearchConditionsBd(); 
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

        private void GRSocketHandler_addEntWell(E_ResState state)
        {
            GRSocketHandler.addEntWell -= GRSocketHandler_addEntWell;
            switch (state)
            {
                case E_ResState.OK:
                    wndMainVM.messageQueueBd.Enqueue("添加企业井信息成功");
                    break;
                case E_ResState.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("添加企业井信息失败");
                    break;
                default:
                    break;
            }
        }
        private void GRSocketHandler_getEntWells(E_ResState state, List<C_EntWell> entWells)
        {
            GRSocketHandler.getEntWells -= GRSocketHandler_getEntWells;
            switch (state)
            {
                case E_ResState.OK:
                    curWellsBd = entWells;
                    GetStateDataByWells();
                    InitMap();
                    pageBarVmBd.Init(entWells);
                    wndMainVM.messageQueueBd.Enqueue("获取企业井信息成功");
                    break;
                case E_ResState.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("获取企业井信息失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_delEntWell(E_ResState state)
        {
            GRSocketHandler.delEntWell -= GRSocketHandler_delEntWell;
            switch (state)
            {
                case E_ResState.OK:
                    refreshCmd(strSearchKeywordBd);
                    wndMainVM.messageQueueBd.Enqueue("删除企业井信息成功");
                    break;                            
                case E_ResState.FAILED:                
                    wndMainVM.messageQueueBd.Enqueue("删除企业井信息失败");
                    break;
                default:
                    break;
            }
        }


        private void GRSocketHandler_edtEntWell(E_ResState state)
        {
            GRSocketHandler.edtEntWell -= GRSocketHandler_edtEntWell;
            switch (state)
            {
                case E_ResState.OK:
                    wndMainVM.SelectPage(E_Page.EntWell_Search_Lst);
                    wndMainVM.messageQueueBd.Enqueue("编辑企业井信息成功");
                    break;                            
                case E_ResState.FAILED:                
                    wndMainVM.messageQueueBd.Enqueue("编辑企业井信息失败");
                    break;
                default:
                    break;
            }
        }


        private void C_EntWellOfcOper_readWell(bool state, int curIndex, int totalCount, List<C_EntWell> wells, string errMsg)
        {
            if (state)
            {
                DispatchService.Invoke(() =>
                {
                    UpdateErrLog(totalCount, curIndex, iErrCount, "success");
                });


                if (errMsg == "Finished")
                {
                    if (iErrCount == 0)
                    {
                        DispatchService.Invoke(() =>
                        {
                            autoAddLogBd.Add("共" + totalCount.ToString() + "条读取完毕，点击开始上传");
                        });
                    }
                    else
                    {
                        DispatchService.Invoke(() =>
                        {
                            autoAddLogBd.Add("共" + totalCount.ToString() + "条读取完毕，请修改不适配项");
                        });

                    }
                    IsReadingFromExcel = false;
                    autoLoadWells = wells;
                    return;
                }
            }
            else
            {
                if (errMsg == "FileNeedToBeClosed")
                {
                    wndMainVM.messageQueueBd.Enqueue("请先关闭文档后重试。");
                    IsReadingFromExcel = false;
                    return;
                }

                iErrCount++;
                DispatchService.Invoke(() =>
                {
                    UpdateErrLog(totalCount, curIndex, iErrCount, errMsg);
                });

            }
        }




        #endregion SocketHandler

        #region Bindings
        public int pageIndexBd { get; set; } = 0;

        //parasettin
        public string strPsLocBd { get; set; }
        public string strPsUnitCatBd { get; set; }
        public string strPsTubeMatBd { get; set; }
        public string strPsPumpModelBd { get; set; }
        public string strPsUseForBd { get; set; }
        public C_WellParas wpBd { get; set; } = C_RT.ewp;
        public C_WellParas wpForSearchBd { get; set; } = new C_WellParas();
        public C_BdAreaCode cbdAcBd { get; set; } = new C_BdAreaCode();
        public C_BdAreaCode ebdAcBd { get; set; } = new C_BdAreaCode();
        public C_EntWell cwBd { get; set; } = new C_EntWell();
        public static C_EntWell ewBd { get; set; } = new C_EntWell();
        public C_BdAreaCode scAcBd { get; set; } = new C_BdAreaCode();
        //search
        public string strSearchKeywordBd { get; set; }
        public List<C_EntWell> curWellsBd { get; set; } = new List<C_EntWell>();
        public C_EntWell curWellIndexBd { get; set; } = new C_EntWell();
        public CtrlEntWellPageBarViewModel pageBarVmBd { get; set; }

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

        public Visibility btnMapVBd { get; set; } = Visibility.Collapsed;
        public Visibility btnEarthVBd { get; set; } = Visibility.Visible;
        public PackIconKind advancedPIKindBd { get; set; } = PackIconKind.ArrowBottomDropCircleOutline;
        public Thickness mapMarginBd { get; set; } = new Thickness(34, 88, 34, 60);
        public Visibility advanceToolBarVBd { get; set; } = Visibility.Collapsed;
        public C_SearchCondition scBd { get; set; } = new C_SearchCondition();
        #endregion Bindings

        #region Actions
       public void ClearSearchConditionsBd()
        {
            wpForSearchBd.LocIndex = wpForSearchBd.Loc.Where(c =>
            {
                if (c.Type == E_WellParaType.Loc && c.Value == "全部")
                    return true;
                else
                    return false;
            }).ToList()[0];
            wpForSearchBd.PumpModelIndex = wpForSearchBd.PumpModel.Where(c =>
            {
                if (c.Type == E_WellParaType.PumpModel && c.Value == "全部")
                    return true;
                else
                    return false;
            }).ToList()[0];
            wpForSearchBd.UnitCatIndex = wpForSearchBd.UnitCat.Where(c =>
            {
                if (c.Type == E_WellParaType.UnitCat && c.Value == "全部")
                    return true;
                else
                    return false;
            }).ToList()[0];
            wpForSearchBd.TubeMatIndex = wpForSearchBd.TubeMat.Where(c =>
            {
                if (c.Type == E_WellParaType.TubeMat && c.Value == "全部")
                    return true;
                else
                    return false;
            }).ToList()[0];
            wpForSearchBd.UseForIndex = wpForSearchBd.UseFor.Where(c =>
            {
                if (c.Type == E_WellParaType.UseFor && c.Value == "全部")
                    return true;
                else
                    return false;
            }).ToList()[0];

            C_AreaCode acTmp4 = new C_AreaCode();
            acTmp4.Id = 0; acTmp4.Level = 4; acTmp4.PCode = 0; acTmp4.Code = 0; acTmp4.Name = "全区";
            C_AreaCode acTmp5 = new C_AreaCode();
            acTmp5.Id = 0; acTmp5.Level = 5; acTmp5.PCode = 0; acTmp5.Code = 0; acTmp5.Name = "全街道（乡镇）";


            scAcBd.L4Index = scAcBd.AllL4AreaCodes.Where(c =>
            {
                if (c.Id == 0 && c.Level == 4 && c.PCode == 0 && c.Code == 0 && c.Name == "全区")
                    return true;
                else
                    return false;
            }
            ).ToList()[0];
            scAcBd.L5Index = scAcBd.AllL5AreaCodes.Where(c =>
            {
                if (c.Id == 0 && c.Level == 5 && c.PCode == 0 && c.Code == 0 && c.Name == "全街道（乡镇）")
                    return true;
                else
                    return false;
            }
            ).ToList()[0];
        }
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
            List<C_EntWell> wells = new List<C_EntWell>();
            cwBd.TsOrSt = cbdAcBd.L4Index.Name;
            cwBd.Loc = wpBd.LocIndex.Value;
            cwBd.TubeMat = wpBd.TubeMatIndex.Value;
            cwBd.UnitCat = wpBd.UnitCatIndex.Value;
            cwBd.PumpMode = wpBd.PumpModelIndex.Value;
            cwBd.Usefor = wpBd.UseForIndex.Value;
            if (CheckCreateWell(cwBd))
            {
                GRSocketHandler.addEntWell += GRSocketHandler_addEntWell;
                wells.Add(cwBd);
                GRSocketAPI.AddEntWells(wells);
            }
        }

        //search
        public void refreshCmd(string keywords)
        {
            scBd.TsOrSt = scAcBd.L4Index.Name;
            scBd.Village = scAcBd.L5Index.Name;
            scBd.UnitCat = wpForSearchBd.UnitCatIndex.Value;
            scBd.PumpMode = wpForSearchBd.PumpModelIndex.Value;
            scBd.UseFor = wpForSearchBd.UseForIndex.Value;
            scBd.TubeMat = wpForSearchBd.TubeMatIndex.Value;
            scBd.Loc = wpForSearchBd.LocIndex.Value;
            scBd.Key = keywords;
            GRSocketHandler.getEntWells += GRSocketHandler_getEntWells;
            GRSocketAPI.GetEntWells(scBd);
        }
        public bool CanedtWellCmd => (curWellIndexBd != null);
        public void edtWellCmd()
        {
            ewBd = curWellIndexBd;
            wndMainVM.SelectPage(E_Page.EntWell_Edit);
        }

        public bool CandelWellCmd => (curWellIndexBd != null);
        public void delWellCmd()
        {
            GRSocketHandler.delEntWell += GRSocketHandler_delEntWell;
            GRSocketAPI.DelEntWell(curWellIndexBd.Id);
        }
        public void saveEdtWellCmd()
        {
            ewBd.TsOrSt = ebdAcBd.L4Index.Name;
            ewBd.Loc = wpBd.LocIndex.Value;
            ewBd.TubeMat = wpBd.TubeMatIndex.Value;
            ewBd.UnitCat = wpBd.UnitCatIndex.Value;
            ewBd.PumpMode = wpBd.PumpModelIndex.Value;
            ewBd.Usefor = wpBd.UseForIndex.Value;
            if (CheckCreateWell(ewBd))
            {
                GRSocketHandler.edtEntWell += GRSocketHandler_edtEntWell;
                GRSocketAPI.EdtEntWell(ewBd);
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
            C_EntWellOfcOper.readWell += C_EntWellOfcOper_readWell;
            C_EntWellOfcOper.ReadWellsFromFile(inputFilePathBd);
        }
        public void loadWellToSvrCmd()
        {
            GRSocketHandler.addEntWell += GRSocketHandler_addEntWell;
            GRSocketAPI.AddEntWells(autoLoadWells);
        }

        public void openTemplateCmd()
        {
            C_EntWellOfcOper.OpenInputTemplete();
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
            C_EntWellOfcOper.OutputWell(opBd, curWellsBd);
        }

        public void SwitchMapCmd(string index)
        {
            if(index == "0")
            {
                if(mapBd.Map.Layers.Contains(bingMap))
                    mapBd.Map.Layers.Remove(bingMap);
                if(!mapBd.Map.Layers.Contains(osmMap))
                    mapBd.Map.Layers.Add(osmMap);
                mapBd.Map.Layers.Move(0,osmMap);
                btnMapVBd = Visibility.Collapsed;
                btnEarthVBd = Visibility.Visible;
            }
            else
            {
                if(mapBd.Map.Layers.Contains(osmMap))
                    mapBd.Map.Layers.Remove(osmMap); 
                if(!mapBd.Map.Layers.Contains(bingMap))
                    mapBd.Map.Layers.Add(bingMap);
                mapBd.Map.Layers.Move(0,bingMap);
                btnMapVBd = Visibility.Visible;
                btnEarthVBd = Visibility.Collapsed;
            }
        }
        public bool isAdvancedSearching { get; set; } = false;
        public void advancedSearchCmd()
        {
            isAdvancedSearching = !isAdvancedSearching;
            if (isAdvancedSearching)
            {
                advancedPIKindBd = PackIconKind.ArrowTopDropCircleOutline;
                mapMarginBd = new Thickness(34, 146, 34, 60);
                advanceToolBarVBd = Visibility.Visible;
            }
            else
            {
                advancedPIKindBd = PackIconKind.ArrowBottomDropCircleOutline;
                mapMarginBd = new Thickness(34, 88, 34, 60);
                advanceToolBarVBd = Visibility.Collapsed;                
                ClearSearchConditionsBd();
                refreshCmd(""); 
            }
        }

        #endregion Actions
        public bool isWaitingForRefreshParas { get; set; } = false;

        TileLayer osmMap = new TileLayer(KnownTileSources.Create(KnownTileSource.OpenStreetMap)) { Name = "OpenStreetMap" };
        TileLayer bingMap = new TileLayer(KnownTileSources.Create(KnownTileSource.BingAerial)) { Name = "Bing Aerial" };
        public static ShapeFile bdarySource = new ShapeFile(System.Environment.CurrentDirectory + "\\Resource\\Shp\\Baodi District_AL6.shp", true){ CRS = "EPSG:4326" };
        public RasterizingLayer boundaryLayer { get; set; } = new RasterizingLayer(CreateCountryLayer(bdarySource));

        public bool CheckCreateWell(C_EntWell w)
        {
            if (w.TsOrSt == "")
            {
                wndMainVM.messageQueueBd.Enqueue("请选择街道乡镇");
                return false;
            }
            if (w.EntName == "")
            {
                wndMainVM.messageQueueBd.Enqueue("请输入所属企业");
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

        //loc
        public void InitMap()
        {
            if (pageIndexBd != 5)
                return;
            App.Current.Dispatcher.Invoke((Action)(() =>
            { 
                mapBd = new MapControl();
                mapBd.Map.Layers.Clear();
                SwitchMapCmd("0");
                mapBd.Map.Layers.Add(CreateWellLayer());
                ShowBoundary();

                var centerOfBD = new Mapsui.Geometries.Point(117.309716, 39.717173);
                var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfBD.X, centerOfBD.Y);
                mapBd.Map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, mapBd.Map.Resolutions[12]);
            }));
        }
        private static ILayer CreateCountryLayer(IProvider countrySource)
        {
            return new Layer
            {
                Name = "Baodi District_AL6",
                DataSource = countrySource,
                Style = CreateCountryTheme(),
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
        }
        public void ShowBoundary(bool bShow = true)
        {
            if(bShow)
            {
                if(!mapBd.Map.Layers.Contains(boundaryLayer))
                    mapBd.Map.Layers.Add(boundaryLayer);
                mapBd.Map.Layers.Move(1, boundaryLayer);
            }
            else
            {
                if (mapBd.Map.Layers.Contains(boundaryLayer))
                    mapBd.Map.Layers.Remove(boundaryLayer);
            }
        }

        private static IThemeStyle CreateCountryTheme()
        {
            //Set a gradient theme on the countries layer, based on Population density
            //First create two styles that specify min and max styles
            //In this case we will just use the default values and override the fill-colors
            //using a colorblender. If different line-widths, line- and fill-colors where used
            //in the min and max styles, these would automatically get linearly interpolated.
            var min = new VectorStyle { Outline = new Pen { Color = Color.FromArgb(40, 0, 0, 0) } };
            var max = new VectorStyle { Outline = new Pen { Color = Color.FromArgb(40, 0, 0, 0) } };

            //Create theme using a density from 0 (min) to 400 (max)
            return new GradientTheme("PopDens", 0, 400, min, max) { FillColorBlend = ColorBlend.TwoColors(Color.FromArgb(40, 0, 0, 0) ,Color.FromArgb(40, 0, 0, 0)) };
        }


        private MemoryLayer CreateWellLayer()
        {
            MemoryLayer ml = new MemoryLayer();
            ml.Name = "Point";
            ml.IsMapInfoLayer = true;
            MemoryProvider mp = new MemoryProvider(GetCitiesFromEmbeddedResource());
            ml.DataSource = mp;
            ml.Style = CreateBitmapStyle();
            return ml;
        }
        private SymbolStyle CreateBitmapStyle()
        {
            var path = @"GRCLNT.Resource.Image.loc.png";
            var bitmapId = GetBitmapIdForEmbeddedResource(path);
            var bitmapHeight = 64; // To set the offset correct we need to know the bitmap height
            return new SymbolStyle { BitmapId = bitmapId, SymbolScale = 0.20, SymbolOffset = new Offset(0, bitmapHeight * 0.5) };
        }
        private static int GetBitmapIdForEmbeddedResource(string imagePath)
        {
            var assembly = typeof(PageWellViewModel).GetTypeInfo().Assembly;
            var image = assembly.GetManifestResourceStream(imagePath);
            return BitmapRegistry.Instance.Register(image);
        }
        private IEnumerable<IFeature> GetCitiesFromEmbeddedResource()
        {
            return curWellsBd.Select(c =>
            {
                var feature = new Feature();
                if (c.Lng != "" && c.Lat != "")
                {
                    var point = SphericalMercator.FromLonLat(double.Parse(c.Lng), double.Parse(c.Lat));
                    feature.Geometry = point;
                }
                return feature;
            });
        }
        public bool IsReadingFromExcel { get; set; } = false;
        public int iErrCount = 0;
        private void UpdateErrLog(int a, int c, int ce, string em)
        {
            vErrLogBd = Visibility.Visible;
            txtReadAutoInputingBd = "正在读取" + c.ToString() + "/" + a.ToString() + "(失败" + iErrCount.ToString() + "条）";

            vInputProgBarBd = Convert.ToInt32((float)c / (float)a * 100.0);

            if (autoAddLogBd.Count < 5000 && em != "" && em != "success")
                autoAddLogBd.Add("第" + c.ToString() + "条读取失败，错误信息：" + em);

        }
        public List<C_EntWell> autoLoadWells { get; set; } = new List<C_EntWell>();

        public void GetStateDataByWells()
        {
            if (pageIndexBd != 8)
                return;

            //tsorst
            var tsOrStState = from w in curWellsBd
                               orderby w.TsOrSt
                               group w by w.TsOrSt into TsOrStGroup
                               select new { lb = TsOrStGroup.Key ,cnt = TsOrStGroup .Count()};
            tsWellCntLabelBd.Clear();
            tsWellCntBd.Clear();
            foreach (var t in tsOrStState)
            {
                tsWellCntLabelBd.Add(t.lb);
                tsWellCntBd.Add(new ObservableValue(t.cnt));
            }

            //useForWellCntBd
            var useForState = from w in curWellsBd
                              orderby w.Usefor
                              group w by w.Usefor into UseforGroup
                              select new { lb = UseforGroup.Key, cnt = UseforGroup.Count() };
            useForSeriesBd.Clear();
            foreach (var t in useForState)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    List<int> c = new List<int>();
                    c.Add(t.cnt);
                    PieSeries p = new PieSeries();
                    p.Title = t.lb;
                    p.Values = new ChartValues<int>(c);
                    p.DataLabels = true;
                    useForSeriesBd.Add(p);
                }));
            }
            
            //tubemat
            tubeMatSeriesBd.Clear();
            var tubeMatState = from w in curWellsBd
                              orderby w.TubeMat
                              group w by w.TubeMat into TubeMatGroup
                               select new { lb = TubeMatGroup.Key, cnt = TubeMatGroup.Count() };
            foreach (var t in tubeMatState)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    List<int> c = new List<int>();
                    c.Add(t.cnt);
                    PieSeries p = new PieSeries();
                    p.Title = t.lb;
                    p.Values = new ChartValues<int>(c);
                    p.DataLabels = true;
                    tubeMatSeriesBd.Add(p);
                }));
            }
        }

    }
}
