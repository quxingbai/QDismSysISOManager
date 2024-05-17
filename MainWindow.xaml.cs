using QDismSysISOManager.Utils;
using QDismSysISOManager.Utils.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class FileViewMode
        {
            public FileInfo File { get; set; }
            public long SizeMB => File.Length / 1024 / 1024;
        }
        private DismCommandExecute Executer = new DismCommandExecute();
        public ObservableCollection<FileViewMode> SourceFiles = new ObservableCollection<FileViewMode>();
        public MainWindow()
        {
            InitializeComponent();
            //var cmd = new SplitImageCommand("B:\\OSS\\install_win10_new.esd", "B:\\OSS\\creater\\splittest\\asd.swm", 3000);
            Executer.Output += Executer_Output;
            LIST_SourceFiles.ItemsSource = SourceFiles;
            Drop += MainWindow_Drop;
        }

        private void Executer_Output(DismCommandExecute arg1, string arg2)
        {
            Dispatcher.Invoke(() =>
            {
                CMD.WriteLine(arg2);
            });
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fs = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
                foreach (var f in fs)
                {
                    FileInfo file = new FileInfo(f);
                    var ex = file.Extension;
                    if (ex == ".wim" || ex == ".esd" || ex == ".swm")
                    {
                        SourceFiles.Add(new FileViewMode() { File = file });
                    }
                }
            }
        }

        public void ExecuteCommand(DismCommand cmd)
        {
            CMD.WriteLine("——————————————————————————————————————————");
            CMD.WriteLine("执行 " + cmd.ToString());
            Executer.WriteCommand(cmd);
        }
        protected override void OnClosed(EventArgs e)
        {
            Executer.Dispose();
            base.OnClosed(e);
        }
        private void FILE_MENUITEM_Click(object sender, RoutedEventArgs e)
        {
            FileViewMode md = (sender as MenuItem).DataContext as dynamic;
            switch ((sender as MenuItem).Tag.ToString())
            {
                case "Peek":
                    {
                        Executer.WriteCommandNoWait(new GetImageInfoCommand(md.File.FullName));
                    }; break;
                case "AddToTimeLine":
                    {

                    }; break;
            }
        }

        private void START_Click(object sender, RoutedEventArgs e)
        {
            START.IsEnabled = false;
            var datas = this.TIMELINE.GetTimeLintTaskItems();

            Task.Run(() =>
            {
                int counter = 0, total = datas.Count();
                foreach (var i in datas)
                {
                    Dispatcher.Invoke(() =>
                    {

                        CMD.Clear();
                        CMD.WriteLine((++counter) + "/" + total+" ");
                        CMD.WriteLine(i.CmdType + "开始");
                        CMD.WriteLine(i.Title);
                        CMD.WriteLine("");
                        i.ExecuteStart();
                    });
                    Executer.WriteCommand(i.Command);
                    Dispatcher.Invoke(() =>
                    {
                        i.ExecuteEnd();
                    });
                }
                Dispatcher.Invoke(() =>
                {
                    foreach (var item in datas)
                    {
                        item.ExecuteDefaultTo();
                    }
                    CMD.WriteLine("完成所有任务——>>" + datas.Count());
                    START.IsEnabled = true;
                });
            });
        }

        private void ListBoxItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileViewMode md = (sender as ListBoxItem).DataContext as dynamic;
            CMD.Clear();
            Executer.WriteCommandNoWait(new GetImageInfoCommand(md.File.FullName));
        }

        private void ListBoxItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var d = new DataObject();
            d.SetData(typeof(FileViewMode).Name, (sender as dynamic).DataContext);
            DragDrop.DoDragDrop(this, d, DragDropEffects.Copy);
        }
    }
}
