using GRModel;
using GRSocket;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRCLNT
{
    public class PageDashboardViewModel : Screen
    {
        public PageDashboardViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;

            GRSocketHandler.getLogs += GRSocketHandler_getLogs;
            GRSocketAPI.GetLogs();
            logsBd = new List<C_Log>();
        }


        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler
        private void GRSocketHandler_getLogs(GRModel.E_ResState state, List<GRModel.C_Log> logs)
        {
            GRSocketHandler.getLogs -= GRSocketHandler_getLogs;
            switch (state)
            {
                case E_ResState.OK:
                    logsBd = logs.Take(20).ToList();
                    wndMainVM.messageQueueBd.Enqueue("获取日志信息成功");
                    break;                            
                case E_ResState.FAILED:                
                    wndMainVM.messageQueueBd.Enqueue("获取日志信息失败");
                    break;
                default:
                    break;
            }
        }

        #endregion SocketHandler

        #region Bindings

        public List<C_Log> logsBd { get; set; }
        #endregion Bindings

        #region Actions

        #endregion Actions
    }
}
