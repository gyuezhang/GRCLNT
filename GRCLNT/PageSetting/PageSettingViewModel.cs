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
    public class PageSettingViewModel : Screen
    {
        public PageSettingViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
        }
        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler

        private void GRSocketHandler_edtUser(RES_STATE state)
        {
            GRSocketHandler.edtUser -= GRSocketHandler_edtUser;
            switch (state)
            {
                case RES_STATE.OK:
                    wndMainVM.messageQueueBd.Enqueue("修改用户信息成功");
                    break;
                case RES_STATE.FAILED:
                    userBd = C_RT.user;
                    wndMainVM.messageQueueBd.Enqueue("修改用户信息失败");
                    break;
                default:
                    break;
            }
        }

        #endregion SocketHandler

        #region Bindings
        public int pageIndexBd { get; set; } = 0;
        public C_User userBd { get; set; } = C_RT.user;

        public string edtTelBd { get; set; }
        public string edtEmailBd { get; set; }

        #endregion Bindings

        #region Actions
        public void SelectPageCmd(string cmdPara)
        {
            wndMainVM.SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }

        public void btnEdtUserCmd()
        {
            GRSocketHandler.edtUser += GRSocketHandler_edtUser;
            userBd.Tel = edtTelBd;
            userBd.Email = edtEmailBd;
            GRSocketAPI.EdtUser(userBd);
        }

        public void btnJumpToEdtUserInfoCmd()
        {
            SelectPageCmd("Setting_EdtUserInfo");
            edtTelBd = userBd.Tel;
            edtEmailBd = userBd.Email;
        }

        #endregion Actions
    }
}
