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
using static System.Net.Mime.MediaTypeNames;

namespace QDismSysISOManager
{
    /// <summary>
    /// FakeCmd.xaml 的交互逻辑
    /// </summary>
    public partial class FakeCmd : UserControl
    {
        public FakeCmd()
        {
            InitializeComponent();
        }

        public void WriteLine(String Text)
        {
            TEXT.Text += Text + "\n";
            TEXT.ScrollToEnd();
        }
        public void Clear()
        {
            TEXT.Text = "";
        }
    }
}
