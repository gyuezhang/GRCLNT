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
            edtTelBd = userBd.Tel;
            edtEmailBd = userBd.Email;
        }
        private WndMainViewModel wndMainVM { get; set; }

        #region SocketHandler

        private void GRSocketHandler_edtUser(E_ResState state, C_User user)
        {
            GRSocketHandler.edtUser -= GRSocketHandler_edtUser;
            switch (state)
            {
                case E_ResState.OK:
                    C_RT.user = user;
                    wndMainVM.messageQueueBd.Enqueue("修改用户信息成功"); 
                    SelectPageCmd("Setting_UserInfo");
                    break;
                case E_ResState.FAILED:
                    wndMainVM.messageQueueBd.Enqueue("修改用户信息失败");
                    break;
                default:
                    break;
            }
            userBd = C_RT.user;
        }

        private void GRSocketHandler_resetPwd(E_ResState state)
        {
            GRSocketHandler.resetPwd -= GRSocketHandler_resetPwd;
            switch (state)
            {
                case E_ResState.OK:
                    wndMainVM.messageQueueBd.Enqueue("重置密码成功，请尽快重新登录");
                    SelectPageCmd("Setting_UserInfo");
                    break;                            
                case E_ResState.FAILED:                
                    wndMainVM.messageQueueBd.Enqueue("重置密码失败");
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

        public string oldPwd { get; set; }
        public string newPwd { get; set; }
        public string newPwd2 { get; set; }

        #endregion Bindings

        #region Actions
        public void SelectPageCmd(string cmdPara)
        {
            wndMainVM.SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }

        public void btnEdtUserCmd()
        {
            if (userBd.Tel == edtTelBd && userBd.Email == edtEmailBd)
            {
                SelectPageCmd("Setting_UserInfo");
                return;
            }

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

        public void btnResetPwdCmd()
        {
            GRSocketHandler.resetPwd += GRSocketHandler_resetPwd;

            if(oldPwd == null || oldPwd == "")
            {
                wndMainVM.messageQueueBd.Enqueue("旧密码为空");
                return;
            }


            if (newPwd == null || newPwd == "")
            {
                wndMainVM.messageQueueBd.Enqueue("新密码为空");
                return;
            }

            if (newPwd2 == null || newPwd2 == "" || newPwd != newPwd2)
            {
                wndMainVM.messageQueueBd.Enqueue("新密码不一致");
                return;
            }

            GRSocketAPI.ResetPwd(C_Md5.GetHash(oldPwd), C_Md5.GetHash(newPwd));
        }

        #endregion Actions
    }
}
