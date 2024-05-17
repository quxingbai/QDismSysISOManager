using System;
using System.Collections.Generic;
using System.IO;
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

namespace QDismSysISOManager
{
    /// <summary>
    /// DismTaskCreatePanel.xaml 的交互逻辑
    /// </summary>
    public partial class DismTaskCreatePanel : UserControl
    {
        public enum SubmitType
        {
            Append, Split, Convert
        }
        public event Action<SubmitType, dynamic> Submit;
        public event Action Close;
        public DismTaskCreatePanel()
        {
            InitializeComponent();
        }

        private void BT_Submit_Click(object sender, RoutedEventArgs e)
        {
            TestCreate(TEXT_Append_Target.Text);
            TestCreate(TEXT_Convert_Target.Text);
            TestCreate(TEXT_Split_Target.Text);
            if (EXPORT.IsSelected)
            {
                Submit?.Invoke(SubmitType.Append, new { Ids = TEXT_Append_Indexs.Text.Split(' ').Select(s => int.Parse(s)), Save = TEXT_Append_Target.Text });
            }
            else if (SPLIT.IsSelected)
            {
                Submit?.Invoke(SubmitType.Split, new { Size = TEXT_Split_Size.Text, Save = TEXT_Split_Target.Text });
            }
            else if (CONVERT.IsSelected)
            {
                Submit?.Invoke(SubmitType.Convert, new { Wim = this.CONVERT_Check_Wim.IsChecked, Esd = this.CONVERT_Check_Esd.IsChecked, Save = TEXT_Convert_Target.Text, Ids = TEXT_Convert_Indexs.Text.Split(' ').Select(s => int.Parse(s)) });
            }
            Close?.Invoke();
        }
        private void TestCreate(string path)
        {
            new FileInfo(path).Directory.Create();
        }
    }
}
