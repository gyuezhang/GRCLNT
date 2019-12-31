using System;
using System.Windows;
using GRSocket;
using GRUtil;
using MaterialDesignThemes.Wpf;
using Stylet;

namespace GRCLNT
{
    public class WndLoginViewModel : Screen
    {
        private IWindowManager _windowManager;

        public WndLoginViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        #region SocketHandler

        #endregion SocketHandler

        #region Bindings

        public SnackbarMessageQueue messageQueueBd { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(0.6));
        public int pageIndexBd { get; set; } = 0;
        public string curPwdBd { get; set; }
        public WindowState windowStateBd { get; set; }

        #endregion Bindings

        #region Actions

        public bool CanSettingCmd => pageIndexBd == 0;
        public void SettingCmd()
        {
            pageIndexBd = 1;
        }

        public void LoginCmd()
        {
        
        }

        public void TestLinkCmd()
        {

        }

        public void BackCmd()
        {
            pageIndexBd = 0;
        }

        public void QuitCmd()
        {
            this.RequestClose();
        }

        #endregion Actions
    }
}
