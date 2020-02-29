﻿using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps.Packaging;

namespace GRCLNT
{
    public class PageGwDynaViewModel : Screen
    {
        public PageGwDynaViewModel(WndMainViewModel _wndMainVM)
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
        public void SelectPageCmd(string cmdPara)
        {
            wndMainVM.SelectPage((E_Page)Enum.Parse(typeof(E_Page), cmdPara, true));
        }
        #endregion Actions


    }
}
