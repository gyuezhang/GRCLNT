using System;
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
    /// PageEntWell_Setting_View.xaml 的交互逻辑
    /// </summary>
    public partial class PageEntWell_Setting_View : UserControl
    {
        public PageEntWell_Setting_View()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private void ListViewLoc_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            double d = lv.ActualHeight;
            svLoc.ScrollToVerticalOffset(d);
        }
        private void ListViewTubeMat_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            double d = lv.ActualHeight;
            svTubeMat.ScrollToVerticalOffset(d);
        }
        private void ListViewPumpModel_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            double d = lv.ActualHeight;
            svPumpModel.ScrollToVerticalOffset(d);
        }
        private void ListViewUnitCat_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            double d = lv.ActualHeight;
            svUnitCat.ScrollToVerticalOffset(d);
        }
        private void ListViewUseFor_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            double d = lv.ActualHeight;
            svUseFor.ScrollToVerticalOffset(d);
        }
    }
}
