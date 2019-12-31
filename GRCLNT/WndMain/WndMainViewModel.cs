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
        }


        #region SocketHandler

        #endregion SocketHandler

        #region Bindings

        public WindowState CurWindowState { get; set; }
        public double maxHeightBd { get; set; } = SystemParameters.WorkArea.Height + 7;
        public double maxWidthBd { get; set; } = SystemParameters.WorkArea.Width + 7;
        public Thickness marginBd { get; set; } = new Thickness(0, 0, 0, 0);
        public Visibility vImageDragVisible { get; set; } = Visibility.Visible;
        public string avaLetterBd { get; set; } = C_RT.user.Name.Substring(0, 1).ToUpper();

        #endregion Bindings

        #region Actions

        public void stateChangeCmd()
        {
            //重置最大窗口尺寸（此处避免运行过程中任务栏显隐）
            maxHeightBd = SystemParameters.WorkArea.Height + 7;
            maxWidthBd = SystemParameters.WorkArea.Width + 7;
            marginBd = (CurWindowState == WindowState.Maximized) ? new Thickness(7, 7, 0, 0) : new Thickness(0, 0, 0, 0);

            vImageDragVisible = (CurWindowState != WindowState.Maximized) ? Visibility.Visible : Visibility.Hidden;
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

        #endregion Actions

    }
}
