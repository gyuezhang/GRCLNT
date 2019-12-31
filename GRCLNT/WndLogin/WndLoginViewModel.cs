using System;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using GRModel;
using GRSocket;
using GRUtil;
using MaterialDesignThemes.Wpf;
using Stylet;

namespace GRCLNT
{
    public class WndLoginViewModel : Screen
    {
        private IWindowManager _windowManager;
        private static int iPwdChangeCnt { get; set; } = 0;
        private static bool bFirstLogin { get; set; } = true;
        private DispatcherTimer TimerLoginSuccess = new DispatcherTimer();

        public WndLoginViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;

            cfgBd = CLNTCfg.Get();

            if (cfgBd.RecordPwd)
                curPwdBd = "11111111";
            else
                curPwdBd = "";

            GRSocketHandler.ConnState += GRSocketHandler_ConnState;
            GRSocketHandler.login += GRSocketHandler_login;

            GRSocket.GRSocket.Conn(cfgBd.SvrIp);

            TimerLoginSuccess.Tick += TimerLoginSuccess_Tick;

            CheckAutoLogin();


        }

        private void TimerLoginSuccess_Tick(object sender, System.EventArgs e)
        {
            messageQueueBd = new SnackbarMessageQueue(TimeSpan.FromSeconds(0.6));
            TimerLoginSuccess.Stop();
            ///注册服务器接口返回处理函数
            GRSocketHandler.ConnState -= GRSocketHandler_ConnState;
            GRSocketHandler.login -= GRSocketHandler_login;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                var wndMainViewModel = new WndMainViewModel(_windowManager);
                this._windowManager.ShowWindow(wndMainViewModel);
                this.RequestClose();
            }));
        }

        #region SocketHandler

        private void GRSocketHandler_ConnState(RES_STATE state)
        {
            switch (state)
            {
                case RES_STATE.OK:
                    messageQueueBd.Enqueue("已成功连接到服务器");
                    break;
                case RES_STATE.FAILED:
                    break;
                default:
                    break;
            }
        }

        private void GRSocketHandler_login(RES_STATE state, C_User user)
        {
            switch (state)
            {
                case RES_STATE.OK:
                    C_RT.user = user;
                    LoginSuccess();
                    break;
                case RES_STATE.FAILED:
                    messageQueueBd = new SnackbarMessageQueue(TimeSpan.FromSeconds(0.6));
                    messageQueueBd.Enqueue("用户名或密码错误");
                    break;
                default:
                    break;
            }
        }

        #endregion SocketHandler

        #region Bindings

        public SnackbarMessageQueue messageQueueBd { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(0.6));
        public int pageIndexBd { get; set; } = 0;
        public string curPwdBd { get; set; }
        public WindowState windowStateBd { get; set; }
        public C_Cfg cfgBd { get; set; }
        #endregion Bindings

        #region Actions

        public bool CanSettingCmd => pageIndexBd == 0;
        public void SettingCmd()
        {
            pageIndexBd = 1;
        }

        public void LoginCmd()
        {
            ManualLogin();
        }

        public void TestLinkCmd()
        {
            if (CheckIp())
            {
                messageQueueBd.Enqueue("正在连接到服务器");
                GRSocket.GRSocket.Conn(cfgBd.SvrIp);
            }
        }

        public void BackCmd()
        {
            pageIndexBd = 0;
        }

        public void QuitCmd()
        {
            this.RequestClose();
        }

        public void pwdChangedCmd()
        {
            iPwdChangeCnt++;
        }

        public void stateChangeCmd()
        {
            if(windowStateBd == WindowState.Maximized)
            {
                windowStateBd = WindowState.Normal;
            }
        }

        #endregion Actions

        public void ManualLogin()
        {
            messageQueueBd.Enqueue("正在登录");
            if (iPwdChangeCnt == 1)
                GRSocketAPI.Login(cfgBd.Name, cfgBd.Pwd);
            else
                GRSocketAPI.Login(cfgBd.Name, C_Md5.GetHash(curPwdBd));
        }

        private void CheckAutoLogin()
        {
            if (cfgBd.AutoLogin && bFirstLogin)
                AutoLogin();
        }

        public void AutoLogin()
        {
            messageQueueBd = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));
            messageQueueBd.Enqueue("正在自动登录",
                "取消",
                CancelAutoLogin);
            GRSocketAPI.Login(cfgBd.Name, cfgBd.Pwd);
        }

        public void CancelAutoLogin()
        {
            messageQueueBd = new SnackbarMessageQueue(TimeSpan.FromSeconds(0.6));
            TimerLoginSuccess.Stop();
            messageQueueBd.Enqueue("自动登录已取消");
        }

        public void LoginSuccess()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (iPwdChangeCnt > 1)
                    cfgBd.Pwd = curPwdBd;

                CLNTCfg.Set(cfgBd);
                messageQueueBd.Enqueue("登录成功");
                bFirstLogin = false;
                TimerLoginSuccess.Interval = new TimeSpan(0, 0, 0, 3);
                TimerLoginSuccess.Start();
            }));
        }

        private bool CheckIp()
        {
            if (cfgBd.SvrIp == "")
            {
                messageQueueBd.Enqueue("IP地址不能为空");
                return false;
            }
            try
            {
                IPAddress.Parse(cfgBd.SvrIp);
            }
            catch
            {
                messageQueueBd.Enqueue("无效的IP地址");
                return false;
            }
            return true;
        }
    }
}
