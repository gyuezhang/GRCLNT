using GRModel;
using GRSocket;
using GRUtil;
using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GRCLNT
{
    public class WndMainViewModel : Screen
    {
        private IWindowManager _windowManager;

        public WndMainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            addrsBarVmBd = new CtrlAddrsBarViewModel(this);
            SelectPage(E_Page.HomePage_Dashboard);

            //重置最大窗口尺寸（此处避免运行过程中任务栏显隐）
            maxHeightBd = SystemParameters.WorkArea.Height + 7;
            maxWidthBd = SystemParameters.WorkArea.Width + 7;



            GRSocketHandler.getAreaCodes += GRSocketHandler_getAreaCodes;
            GRSocketAPI.GetAreaCodes();
            GRSocketHandler.getWellParas += GRSocketHandler_getWellParas;
            GRSocketAPI.GetWellParas();
            GRSocketHandler.getEntWellParas += GRSocketHandler_getEntWellParas;
            GRSocketAPI.GetEntWellParas();
        }

        #region SocketHandler

        private void GRSocketHandler_getAreaCodes(E_ResState state, List<C_AreaCode> acs)
        {
            GRSocketHandler.getAreaCodes -= GRSocketHandler_getAreaCodes;
            switch (state)
            {
                case E_ResState.OK:
                    C_RT.acs = acs;
                    break;
                case E_ResState.FAILED:
                    messageQueueBd.Enqueue("获取区划信息失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_getWellParas(E_ResState state, C_WellParas wps)
        {
            GRSocketHandler.getWellParas -= GRSocketHandler_getWellParas;
            switch (state)
            {
                case E_ResState.OK:
                    C_RT.wp = new C_WellParas(wps.All);
                    break;
                case E_ResState.FAILED:
                    messageQueueBd.Enqueue("获取机井参数失败");
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_getEntWellParas(E_ResState state, C_WellParas wps)
        {
            GRSocketHandler.getEntWellParas -= GRSocketHandler_getEntWellParas;
            switch (state)
            {
                case E_ResState.OK:
                    C_RT.ewp = new C_WellParas(wps.All);
                    break;
                case E_ResState.FAILED:
                    messageQueueBd.Enqueue("获取企业井参数失败");
                    break;
                default:
                    break;
            }
        }

        #endregion SocketHandler

        #region Bindings

        public WindowState windowStateBd { get; set; }
        public double maxHeightBd { get; set; } = SystemParameters.WorkArea.Height + 7;
        public double maxWidthBd { get; set; } = SystemParameters.WorkArea.Width + 7;
        public Thickness marginBd { get; set; } = new Thickness(0, 0, 0, 0);
        public string avaLetterBd { get; set; } = C_RT.user.Name.Substring(0, 1).ToUpper();

        public Visibility dragImgVisibilityBd { get; set; } = Visibility.Visible;
        public Visibility menuBtnVisibilityBd { get; set; } = Visibility.Visible;
        public int menuBtnIndexBd { get; set; } = 1;
        public Visibility settingBtnVisibilityBd { get; set; } = Visibility.Hidden;
        public Screen mainVmBd { get; set; } = new Screen();
        public CtrlAddrsBarViewModel addrsBarVmBd { get; set; }
        public SnackbarMessageQueue messageQueueBd { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1.2));
        public PackIconKind menuSwitchPackIconBd { get; set; } = PackIconKind.ArrowRight;
        public int menuWidthBd { get; set; } = 72;
        public bool is1FocusBd { get; set; } = true;
        public bool is2FocusBd { get; set; } = false;
        public bool is3FocusBd { get; set; } = false;
        public bool is4FocusBd { get; set; } = false;
        public bool is5FocusBd { get; set; } = false;
        public bool is6FocusBd { get; set; } = false;
        public bool is7FocusBd { get; set; } = false;
        public bool is8FocusBd { get; set; } = false;
        public bool is9FocusBd { get; set; } = false;
        public bool is10FocusBd { get; set; } = false;
        #endregion Bindings

        #region Actions

        public void stateChangeCmd()
        {
            //重置最大窗口尺寸（此处避免运行过程中任务栏显隐）
            maxHeightBd = SystemParameters.WorkArea.Height + 7;
            maxWidthBd = SystemParameters.WorkArea.Width + 7;
            marginBd = (windowStateBd == WindowState.Maximized) ? new Thickness(7, 7, 0, 0) : new Thickness(0, 0, 0, 0);
            dragImgVisibilityBd = (windowStateBd != WindowState.Maximized) ? Visibility.Visible : Visibility.Hidden;
        }

        public void logOutCmd()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                var wndLoginViewModel = new WndLoginViewModel(_windowManager);
                this._windowManager.ShowWindow(wndLoginViewModel);
                RequestClose();
            }));
        }

        public void QuitCmd()
        {
            this.RequestClose();
        }

        public void MenuBtnCmd(string cmdPara)
        {
            SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }

        public void MenuSwitchCmd( bool bForceClose = false)
        {
            if (bForceClose)
                bMenuStatus = false;
            else
                bMenuStatus = !bMenuStatus;
            if (bMenuStatus)
            {
                menuSwitchPackIconBd = PackIconKind.ArrowLeft;
                menuWidthBd = 210;
            }
            else
            {
                menuSwitchPackIconBd = PackIconKind.ArrowRight;
                menuWidthBd = 72;
            }
        }

        public void menuLostFocusCmd()
        {
            MenuSwitchCmd(true);
        }
        #endregion Actions

        public void SelectPage(E_Page p)
        {
            menuBtnVisibilityBd = Visibility.Visible;
            settingBtnVisibilityBd = Visibility.Hidden;
            addrsBarVmBd = new CtrlAddrsBarViewModel(this);
            addrsBarVmBd.Update(p);
            switch (p)
            {
                case E_Page.HomePage:
                    menuBtnIndexBd = 1;
                    mainVmBd = new PageHomePageViewModel(this);
                    break;
                case E_Page.HomePage_Dashboard:
                    menuBtnIndexBd = 1;
                    mainVmBd = new PageHomePageViewModel(this);
                    ((PageHomePageViewModel)mainVmBd).pageIndexBd = 1;
                    break;
                case E_Page.Well:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    break;
                case E_Page.Well_AddMtdSel:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 1;
                    break;
                case E_Page.Well_AddManual:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 2;
                    break;
                case E_Page.Well_AddAuto:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 3;
                    break;
                case E_Page.Well_Search:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 4;
                    break;
                case E_Page.Well_Search_Loc:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 5;
                    ((PageWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.Well_Search_Lst:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 6;
                    ((PageWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.Well_Edit:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 7;
                    ((PageWellViewModel)mainVmBd).UpdateEdtParas();
                    break;
                case E_Page.Well_State:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 8;
                    ((PageWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.Well_Output:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 9;
                    ((PageWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.Well_Setting:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    ((PageWellViewModel)mainVmBd).pageIndexBd = 10;
                    break;
                case E_Page.EntWell:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    break;
                case E_Page.EntWell_AddMtdSel:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 1;
                    break;
                case E_Page.EntWell_AddManual:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 2;
                    break;
                case E_Page.EntWell_AddAuto:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 3;
                    break;
                case E_Page.EntWell_Search:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 4;
                    break;
                case E_Page.EntWell_Search_Loc:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 5;
                    ((PageEntWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.EntWell_Search_Lst:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 6;
                    ((PageEntWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.EntWell_Edit:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 7;
                    break;
                case E_Page.EntWell_State:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 8;
                    ((PageEntWellViewModel)mainVmBd).refreshCmd("");
                    break;
                case E_Page.EntWell_Output:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 9;
                    break;
                case E_Page.EntWell_Setting:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 10;
                    break;
                case E_Page.EntWell_FetchWaterId:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
                    ((PageEntWellViewModel)mainVmBd).pageIndexBd = 11;
                    break;
                case E_Page.SediCtrl:
                    menuBtnIndexBd = 7;
                    mainVmBd = new PageSediCtrlViewModel(this);
                    break;
                case E_Page.GwDyna:
                    menuBtnIndexBd = 9;
                    mainVmBd = new PageGwDynaViewModel(this);
                    break;
                case E_Page.GwProj:
                    menuBtnIndexBd = 11;
                    mainVmBd = new PageGwProjViewModel(this);
                    break;
                case E_Page.Hydro:
                    menuBtnIndexBd = 13;
                    mainVmBd = new PageHydroViewModel(this);
                    break;
                case E_Page.Law:
                    menuBtnIndexBd = 15;
                    mainVmBd = new PageLawViewModel(this);
                    break;
                case E_Page.Setting:
                    menuBtnVisibilityBd = Visibility.Hidden;
                    settingBtnVisibilityBd = Visibility.Visible;
                    mainVmBd = new PageSettingViewModel(this);
                    break;
                case E_Page.Setting_UserInfo:
                    menuBtnVisibilityBd = Visibility.Hidden;
                    settingBtnVisibilityBd = Visibility.Visible;
                    mainVmBd = new PageSettingViewModel(this);
                    ((PageSettingViewModel)mainVmBd).pageIndexBd = 1;
                    break;
                case E_Page.Setting_EdtUserInfo:
                    menuBtnVisibilityBd = Visibility.Hidden;
                    settingBtnVisibilityBd = Visibility.Visible;
                    mainVmBd = new PageSettingViewModel(this);
                    ((PageSettingViewModel)mainVmBd).pageIndexBd = 2;
                    break;
                case E_Page.Setting_ResetPwd:
                    menuBtnVisibilityBd = Visibility.Hidden;
                    settingBtnVisibilityBd = Visibility.Visible;
                    mainVmBd = new PageSettingViewModel(this);
                    ((PageSettingViewModel)mainVmBd).pageIndexBd = 3;
                    break;
                case E_Page.Setting_SysSetting:
                    menuBtnVisibilityBd = Visibility.Hidden;
                    settingBtnVisibilityBd = Visibility.Visible;
                    mainVmBd = new PageSettingViewModel(this);
                    ((PageSettingViewModel)mainVmBd).pageIndexBd = 4;
                    break;
                case E_Page.Setting_VersionInfo:
                    menuBtnVisibilityBd = Visibility.Hidden;
                    settingBtnVisibilityBd = Visibility.Visible;
                    mainVmBd = new PageSettingViewModel(this);
                    ((PageSettingViewModel)mainVmBd).pageIndexBd = 5;
                    break;
                default:
                    break;
            }
        }
        public bool bMenuStatus = false;


    }
}
