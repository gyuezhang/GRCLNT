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
using System.Windows.Xps.Packaging;

namespace GRCLNT
{
    /// <summary>
    /// PageGwDyna_DataManual_View.xaml 的交互逻辑
    /// </summary>
    public partial class PageGwDyna_DataManual_View : UserControl
    {
        public PageGwDyna_DataManual_View()
        {
            InitializeComponent();

            dv.Document = new XpsDocument(  System.Environment.CurrentDirectory + "\\Resource\\Docs\\DataManual.xps", System.IO.FileAccess.Read).GetFixedDocumentSequence();
        }
    }
}
