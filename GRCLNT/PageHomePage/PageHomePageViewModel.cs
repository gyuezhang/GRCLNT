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
    public class PageHomePageViewModel : Screen
    {
        public PageHomePageViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;

            GRSocketHandler.getLogs += GRSocketHandler_getLogs;
            GRSocketAPI.GetLogs();
            logsBd = new List<C_ShowLog>();
        }


        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler
        private void GRSocketHandler_getLogs(GRModel.E_ResState state, List<GRModel.C_Log> logs)
        {
            GRSocketHandler.getLogs -= GRSocketHandler_getLogs;
            switch (state)
            {
                case E_ResState.OK:
                    logsBd = GetShowLogs(logs.Take(20).ToList());
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
        public int pageIndexBd { get; set; } = 0;
        public List<C_ShowLog> logsBd { get; set; }
        #endregion Bindings

        #region Actions
        public void SelectPageCmd(string cmdPara)
        {
            wndMainVM.SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }


        #endregion Actions

        public List<C_ShowLog> GetShowLogs(List<C_Log> logs)
        {
            List<C_ShowLog> res = new List<C_ShowLog>();

            foreach(C_Log l in logs)
            {
                C_ShowLog sl = new C_ShowLog();
                sl.UsrName = GetUserNameById(l.UserId);
                sl.Ava = sl.UsrName.Substring(sl.UsrName.Length-1,1);
                sl.Oper = C_Str.GetOperByApiId(l.Api);
                sl.RecordTime = l.Time.ToString("f");
                res.Add(sl);
            }

            return res;
        }

        public string GetUserNameById(int id)
        {
            foreach(C_User u in C_RT.users)
            {
                if (id == u.Id)
                    return u.Name;
            }
            return "X";
        }
    }
}
