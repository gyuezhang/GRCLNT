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

namespace GRCLNT
{
    public class PageWellViewModel : Screen
    {
        public PageWellViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
            wpBd = C_RT.wp;
            cbdAcBd = new C_BdAreaCode(C_RT.acs);
            ebdAcBd = new C_BdAreaCode(C_RT.acs);
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
                    InitMap();
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

        private void GRSocketHandler_edtWell(RES_STATE state)
        {
            GRSocketHandler.edtWell -= GRSocketHandler_edtWell;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.SelectPage(E_Page.Well_Search_Lst);
                    wndMainVM.messageQueueBd.Enqueue("编辑机井信息成功");
                    break;                            
                case RES_STATE.FAILED:                
                    wndMainVM.messageQueueBd.Enqueue("编辑机井信息失败");
                    break;
                default:
                    break;
            }
        }

        private void C_ExcelOper_readWell(bool state, int curIndex, int totalCount, List<C_Well> wells, string errMsg)
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
        public static C_Well ewBd { get; set; } = new C_Well();

        //search
        public string strSearchResBd { get; set; }
        public string strSearchKeywordBd { get; set; }
        public List<C_Well> curWellsBd { get; set; } = new List<C_Well>();
        public C_Well curWellIndexBd { get; set; } = new C_Well();

        //loc
        public MapControl mapBd { get; set; } = new MapControl();

        //auto add
        public string inputFilePathBd { get; set; }
        public ObservableCollection<string> autoAddLogBd { get; set; } = new ObservableCollection<string>();
        public int vInputProgBarBd { get; set; } = 0;
        public Visibility vErrLogBd { get; set; } = Visibility.Collapsed; 
        public Visibility vInputingBd { get; set; } = Visibility.Collapsed;
        public string txtReadAutoInputingBd { get; set; } = "";

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

        //loc
        public void InitMap()
        {
            mapBd.Map.Layers.Clear();
            mapBd.Map.Layers.Add(new TileLayer(KnownTileSources.Create()));
            mapBd.Map.Layers.Add(CreateWellLayer());

            var centerOfBD = new Mapsui.Geometries.Point(117.309716, 39.717173);
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfBD.X, centerOfBD.Y);
            mapBd.Map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, mapBd.Map.Resolutions[12]);

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
        public List<C_Well> autoLoadWells { get; set; } = new List<C_Well>();
    }

    public static class DispatchService
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }
    }

}
