﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GRCLNT
{
    /// <summary>
    /// PageEntWell_AddAuto_View.xaml 的交互逻辑
    /// </summary>
    public partial class PageEntWell_AddAuto_View : UserControl
    {
        public PageEntWell_AddAuto_View()
        {
            InitializeComponent();
        }
        
        private void lv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double d = lv.ActualHeight;
            sv.ScrollToVerticalOffset(d);
        }

        private void sv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

    }
}
