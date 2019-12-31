using MaterialDesignThemes.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GRCLNT
{
    public class WndMainViewModel : Screen
    {
        private IWindowManager _windowManager;

        public WndMainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            SelectPage(E_Page.Dashboard);

            //重置最大窗口尺寸（此处避免运行过程中任务栏显隐）
            maxHeightBd = SystemParameters.WorkArea.Height + 7;
            maxWidthBd = SystemParameters.WorkArea.Width + 7;


            mainVmBd = new PageDashboardViewModel(this);
            addrsBarVmBd = new CtrlAddrsBarViewModel(this);
        }


        #region SocketHandler

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
        public Screen mainVmBd { get; set; }
        public CtrlAddrsBarViewModel addrsBarVmBd { get; set; }
        public SnackbarMessageQueue messageQueueBd { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(1.2));

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

        #endregion Actions

        public void SelectPage(E_Page p)
        {
            menuBtnVisibilityBd = Visibility.Visible;
            settingBtnVisibilityBd = Visibility.Hidden;
            addrsBarVmBd = new CtrlAddrsBarViewModel(this);
            addrsBarVmBd.Update(p);
            switch (p)
            {
                case E_Page.Dashboard:
                    menuBtnIndexBd = 1;
                    mainVmBd = new PageDashboardViewModel(this);
                    break;
                case E_Page.Well:
                    menuBtnIndexBd = 3;
                    mainVmBd = new PageWellViewModel(this);
                    break;
                case E_Page.EntWell:
                    menuBtnIndexBd = 5;
                    mainVmBd = new PageEntWellViewModel(this);
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
                default:
                    break;
            }
        }

    }
}
