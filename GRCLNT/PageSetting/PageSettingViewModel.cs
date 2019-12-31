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

        #endregion SocketHandler

        #region Bindings
        public int pageIndexBd { get; set; } = 0;

        #endregion Bindings

        #region Actions

        #endregion Actions
    }
}
