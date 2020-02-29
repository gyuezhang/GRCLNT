using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GRCLNT
{
    public class CtrlAddrsBarViewModel : Screen
    {
        public CtrlAddrsBarViewModel(WndMainViewModel _wndMainVM)
        {
            wndMainVM = _wndMainVM;
            Update(E_Page.HomePage);
        }
        private WndMainViewModel wndMainVM { get; set; }

        #region Bindings

        public List<C_AddrsBarItem> itemsBd { get; set; } = new List<C_AddrsBarItem>();

        #endregion Bindings

        #region Actions

        public void chipCmd(string pageId)
        {
            try
            {
                E_Page id = (E_Page)Enum.Parse(typeof(E_Page), pageId);
                wndMainVM.SelectPage(id);
            }
            catch
            {

            }
        }

        #endregion Actions

        public void Update(E_Page id)
        {
            itemsBd.Clear();
            switch (id)
            {
                case E_Page.HomePage:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.HomePage));
                    break;
                case E_Page.HomePage_Dashboard:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.HomePage));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.HomePage_Dashboard));
                    break;
                case E_Page.Well:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    break;
                case E_Page.Well_AddMtdSel:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_AddMtdSel));
                    break;
                case E_Page.Well_AddManual:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_AddMtdSel));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_AddManual));
                    break;
                case E_Page.Well_AddAuto:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_AddMtdSel));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_AddAuto));
                    break;
                case E_Page.Well_Search:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Search));
                    break;
                case E_Page.Well_Search_Loc:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Search));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Search_Loc));
                    break;
                case E_Page.Well_Search_Lst:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Search));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Search_Lst));
                    break;
                case E_Page.Well_Edit:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Search));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Edit));
                    break;
                case E_Page.Well_State:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_State));
                    break;
                case E_Page.Well_Output:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Output));
                    break;
                case E_Page.Well_Setting:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Well_Setting));
                    break;
                case E_Page.EntWell:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    break;
                case E_Page.EntWell_AddMtdSel:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_AddMtdSel));
                    break;
                case E_Page.EntWell_AddManual:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_AddMtdSel));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_AddManual));
                    break;
                case E_Page.EntWell_AddAuto:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_AddMtdSel));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_AddAuto));
                    break;
                case E_Page.EntWell_Search:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Search));
                    break;
                case E_Page.EntWell_Search_Loc:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Search));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Search_Loc));
                    break;
                case E_Page.EntWell_Search_Lst:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Search));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Search_Lst));
                    break;
                case E_Page.EntWell_Edit:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Search));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Edit));
                    break;
                case E_Page.EntWell_State:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_State));
                    break;
                case E_Page.EntWell_Output:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Output));
                    break;
                case E_Page.EntWell_Setting:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_Setting));
                    break;
                case E_Page.EntWell_FetchWaterId:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.EntWell_FetchWaterId));
                    break;
                case E_Page.SediCtrl:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.SediCtrl));
                    break;
                case E_Page.GwDyna:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.GwDyna));
                    break;
                case E_Page.GwDyna_DataManual:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.GwDyna));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.GwDyna_DataManual));
                    break;
                case E_Page.GwProj:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.GwProj));
                    break;
                case E_Page.Hydro:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Hydro));
                    break;
                case E_Page.Law:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Law));
                    break;
                case E_Page.Setting:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting));
                    break;
                case E_Page.Setting_UserInfo:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_UserInfo));
                    break;
                case E_Page.Setting_EdtUserInfo:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_UserInfo));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_EdtUserInfo));
                    break;
                case E_Page.Setting_ResetPwd:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_UserInfo));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_ResetPwd));
                    break;
                case E_Page.Setting_SysSetting:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_SysSetting));
                    break;
                case E_Page.Setting_VersionInfo:
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_SysSetting));
                    itemsBd.Add(new C_AddrsBarItem(E_Page.Setting_VersionInfo));
                    break;
                default:
                    break;
            }
            if(itemsBd.Count > 0)
                itemsBd[itemsBd.Count - 1].VHasChild = Visibility.Collapsed;
        }
    }
}
