using Stylet;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace GRCLNT
{
    public enum E_Page
    {
        HomePage,
            HomePage_Dashboard,
        Well,
            Well_AddMtdSel,
                Well_AddManual,
                Well_AddAuto,
            Well_Search,
                Well_Search_Loc,
                Well_Search_Lst,
                Well_Edit,
            Well_State,
            Well_Output,
            Well_Setting,
        EntWell,
            EntWell_AddMtdSel,
                EntWell_AddManual,
                EntWell_AddAuto,
            EntWell_Search,
                EntWell_Search_Loc,
                EntWell_Search_Lst,
                EntWell_Edit,
            EntWell_State,
            EntWell_Output,
            EntWell_Setting,
            EntWell_FetchWaterId,
        SediCtrl,
        GwDyna,
            GwDyna_DataManual,
        GwProj,
        Hydro,
        Law,
        Setting,
            Setting_UserInfo,
                Setting_EdtUserInfo,
                Setting_ResetPwd,
            Setting_SysSetting,
                Setting_VersionInfo,
    }

    public class C_AddrsBarItem : PropertyChangedBase
    {
        public E_Page PageId { get; set; }
        private string _cont;
        private PackIconKind _iconKind;
        private Visibility _vHasChild;
        private string _strPageId;

        public string StrPageId
        {
            get { return _strPageId; }
            set
            {
                SetAndNotify(ref _strPageId, value);
            }
        }

        public string Cont
        {
            get { return _cont; }
            set
            {
                SetAndNotify(ref _cont, value);
            }
        }

        public PackIconKind IconKind
        {
            get { return _iconKind; }
            set
            {
                SetAndNotify(ref _iconKind, value);
            }
        }

        public Visibility VHasChild
        {
            get { return _vHasChild; }
            set
            {
                SetAndNotify(ref _vHasChild, value);
            }
        }

        public C_AddrsBarItem(E_Page id)
        {
            PageId = id;
            Cont = GetPageIdName();
            IconKind = GetIconKind();
            VHasChild = Visibility.Visible;
            StrPageId = id.ToString();
        }

        private string GetPageIdName()
        {
            switch (PageId)
            {
                case E_Page.HomePage:
                    return "首页";
                case E_Page.HomePage_Dashboard:
                    return "仪表板";
                case E_Page.Well:
                    return "机井信息";
                case E_Page.Well_AddMtdSel:
                    return "信息采集";
                case E_Page.Well_AddManual:
                    return "手动录入";
                case E_Page.Well_AddAuto:
                    return "自动导入";
                case E_Page.Well_Search:
                    return "查询";
                case E_Page.Well_Search_Loc:
                    return "位置查询";
                case E_Page.Well_Search_Lst:
                    return "列表查询";
                case E_Page.Well_Edit:
                    return "编辑";
                case E_Page.Well_State:
                    return "统计";
                case E_Page.Well_Output:
                    return "导出";
                case E_Page.Well_Setting:
                    return "机井参数设置";
                case E_Page.EntWell:
                    return "企业井管理";
                case E_Page.EntWell_AddMtdSel:
                    return "信息采集";
                case E_Page.EntWell_AddManual:
                    return "手动录入";
                case E_Page.EntWell_AddAuto:
                    return "自动导入";
                case E_Page.EntWell_Search:
                    return "查询";
                case E_Page.EntWell_Search_Loc:
                    return "位置查询";
                case E_Page.EntWell_Search_Lst:
                    return "列表查询";
                case E_Page.EntWell_Edit:
                    return "编辑";
                case E_Page.EntWell_State:
                    return "统计";
                case E_Page.EntWell_Output:
                    return "导出";
                case E_Page.EntWell_Setting:
                    return "企业井参数设置";
                case E_Page.EntWell_FetchWaterId:
                    return "取水许可";
                case E_Page.SediCtrl:
                    return "控沉工作";
                case E_Page.GwDyna:
                    return "地下水动态";
                case E_Page.GwDyna_DataManual:
                    return "监测数据查询手册";
                case E_Page.GwProj:
                    return "地下水工程";
                case E_Page.Hydro:
                    return "水文地质";
                case E_Page.Law:
                    return "法律法规";
                case E_Page.Setting:
                    return "设置";
                case E_Page.Setting_UserInfo:
                    return "用户信息";
                case E_Page.Setting_EdtUserInfo:
                    return "编辑用户";
                case E_Page.Setting_ResetPwd:
                    return "重置密码";
                case E_Page.Setting_SysSetting:
                    return "系统设置";
                case E_Page.Setting_VersionInfo:
                    return "版本信息";
                default:
                    return "";
            }
        }

        private PackIconKind GetIconKind()
        {
            switch (PageId)
            {
                case E_Page.HomePage:
                    return PackIconKind.Home;
                case E_Page.HomePage_Dashboard:
                    return PackIconKind.ViewDashboard;
                case E_Page.Well:
                    return PackIconKind.WaterPump;
                case E_Page.Well_AddMtdSel:
                    return PackIconKind.Forwardburger;
                case E_Page.Well_AddManual:
                    return PackIconKind.PlaylistEdit;
                case E_Page.Well_AddAuto:
                    return PackIconKind.ScrewMachineFlatTop;
                case E_Page.Well_Search:
                    return PackIconKind.SearchWeb;
                case E_Page.Well_Search_Loc:
                    return PackIconKind.Location;
                case E_Page.Well_Search_Lst:
                    return PackIconKind.ViewList;
                case E_Page.Well_Edit:
                    return PackIconKind.Edit;
                case E_Page.Well_State:
                    return PackIconKind.ChartLine;
                case E_Page.Well_Output:
                    return PackIconKind.PageNextOutline;
                case E_Page.Well_Setting:
                    return PackIconKind.Equaliser;
                case E_Page.EntWell:
                    return PackIconKind.Factory;
                case E_Page.EntWell_AddMtdSel:
                    return PackIconKind.Forwardburger;
                case E_Page.EntWell_AddManual:
                    return PackIconKind.PlaylistEdit;
                case E_Page.EntWell_AddAuto:
                    return PackIconKind.ScrewMachineFlatTop;
                case E_Page.EntWell_Search:
                    return PackIconKind.SearchWeb;
                case E_Page.EntWell_Search_Loc:
                    return PackIconKind.Location;
                case E_Page.EntWell_Search_Lst:
                    return PackIconKind.ViewList;
                case E_Page.EntWell_Edit:
                    return PackIconKind.Edit;
                case E_Page.EntWell_State:
                    return PackIconKind.ChartLine;
                case E_Page.EntWell_Output:
                    return PackIconKind.PageNextOutline;
                case E_Page.EntWell_Setting:
                    return PackIconKind.Equaliser;
                case E_Page.EntWell_FetchWaterId:
                    return PackIconKind.IdCard;
                case E_Page.SediCtrl:
                    return PackIconKind.Layers;
                case E_Page.GwDyna:
                    return PackIconKind.Water;
                case E_Page.GwDyna_DataManual:
                    return PackIconKind.FileWord;
                case E_Page.GwProj:
                    return PackIconKind.Worker;
                case E_Page.Hydro:
                    return PackIconKind.Map;
                case E_Page.Law:
                    return PackIconKind.ScaleBalance;
                case E_Page.Setting:
                    return PackIconKind.Settings;
                case E_Page.Setting_UserInfo:
                    return PackIconKind.AccountBadgeHorizontal;
                case E_Page.Setting_EdtUserInfo:
                    return PackIconKind.AccountDetails;
                case E_Page.Setting_ResetPwd:
                    return PackIconKind.LockReset;
                case E_Page.Setting_SysSetting:
                    return PackIconKind.SettingsTransfer;
                case E_Page.Setting_VersionInfo:
                    return PackIconKind.Tags;
                default:
                    return PackIconKind.Brightness1;
            }
        }

    }
}
