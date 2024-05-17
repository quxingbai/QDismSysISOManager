using QDismSysISOManager.Utils.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
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
using static QDismSysISOManager.MainWindow;

namespace QDismSysISOManager
{
    /// <summary>
    /// TaskTimeLine.xaml 的交互逻辑
    /// </summary>
    public partial class TaskTimeLine : UserControl
    {
        public class TimeLineTaskItem : INotifyPropertyChanged
        {
            public DismCommand Command { get; set; }
            public String Title { get; set; }
            public FileInfo Target { get; set; } = null;
            public FileInfo From { get; set; }
            public DismTaskCreatePanel.SubmitType CmdType { get; set; }
            public TimeLineTaskItem Parent { get; set; }
            public string BigTitle { get; set; }
            public String InfoTitle { get; set; }
            public bool HasParent => Parent != null;

            public bool IsProcessStart { get; set; } = false;
            public bool IsProcesssEnd { get; set; } = false;
            public event PropertyChangedEventHandler? PropertyChanged;
            public void Update()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Command"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Target"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("From"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CmdType"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Parent"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InfoTitle"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BigTitle"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasParent"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcesssEnd"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcessStart"));
            }
            public void ExecuteStart()
            {
                IsProcesssEnd = false;
                IsProcessStart = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcesssEnd"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcessStart"));
            }
            public void ExecuteEnd()
            {
                IsProcesssEnd = true;
                IsProcessStart = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcesssEnd"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcessStart"));
            }
            public void ExecuteDefaultTo()
            {
                IsProcesssEnd = false;
                IsProcessStart = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcesssEnd"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsProcessStart"));
            }

        }
        private ObservableCollection<TimeLineTaskItem> Source = new ObservableCollection<TimeLineTaskItem>();
        public TaskTimeLine()
        {
            InitializeComponent();
            Drop += TaskTimeLine_Drop;
            LIST.ItemsSource = Source;
        }
        public void EditTimeLineItem(TimeLineTaskItem target)
        {
            DismTaskCreatePanel panel = new DismTaskCreatePanel();
            panel.TEXT_Append_Indexs.Text = "1";
            var appendOutput = target.From.Directory.FullName + "\\output\\install.wim"; ;
            panel.TEXT_Append_Target.Text = appendOutput;
            panel.TEXT_Split_Target.Text = target.From.Directory.FullName + "\\output\\splits\\install.swm";


            if (target.From.Extension.ToLower() == ".wim")
            {
                panel.CONVERT_Check_Esd.IsChecked = true;
                panel.CONVERT_Check_Wim.IsEnabled = false;
                panel.TEXT_Convert_Target.Text = target.From.FullName + ".esd";
            }
            else
            {
                panel.CONVERT_Check_Esd.IsEnabled = false;
                panel.CONVERT_Check_Wim.IsChecked = true;
                panel.TEXT_Convert_Target.Text = target.From.FullName + ".wim";
            }

            ShowTopControl(panel);
            panel.Close += () => RemoveTopControl(panel);
            panel.Submit += (type, data) =>
            {
                target.CmdType = type;
                switch (type)
                {
                    case DismTaskCreatePanel.SubmitType.Append:
                        {
                            IEnumerable<int> Ids = data.Ids;
                            string SaveTo = data.Save;
                            int counter = 0;
                            int pos = Source.IndexOf(target);
                            foreach (var i in Ids)
                            {
                                counter++;
                                var title = "合成 " + target.From.Name + "-" + counter + "\n源文件:" + target.From.FullName + "\n" + "输出文件:" + SaveTo + "\n源文件大小:" + FileSizeMBStr(target.From);
                                if (counter == 1)
                                {
                                    target.Command = new ExportAppendImageCommand(target.From.FullName, i, SaveTo);
                                    target.Title = title;
                                    target.Target = new FileInfo(SaveTo);
                                    target.BigTitle = "合成";
                                    target.InfoTitle = "合成[" + i + "]";
                                }
                                else
                                {
                                    Source.Insert(pos + 1, new TimeLineTaskItem()
                                    {
                                        From = target.From,
                                        Command = new ExportAppendImageCommand(target.From.FullName, i, SaveTo),
                                        Title = title,
                                        Target = target.Target,
                                        Parent = target,
                                        BigTitle = "合成",
                                        InfoTitle = "连接合成[" + (i) + "]"
                                    });
                                }
                            }
                        }
                        break;
                    case DismTaskCreatePanel.SubmitType.Split:
                        {
                            long SizeMb = long.Parse(data.Size + "");
                            string SaveTo = data.Save;
                            target.Command = new SplitImageCommand(target.From.FullName, SaveTo, SizeMb);
                            target.Title = "分割 " + target.From.Name + "\n分割大小:" + SizeMb + "\n源文件:" + target.From.FullName + "\n" + "输出文件:" + SaveTo + "\n源文件大小:" + FileSizeMBStr(target.From);
                            target.Target = new FileInfo(SaveTo);
                            target.BigTitle = "分割";
                            target.InfoTitle = "分割 " + SizeMb + "MB";
                        }
                        break;

                    case DismTaskCreatePanel.SubmitType.Convert:
                        {
                            IEnumerable<int> ids = data.Ids;
                            string SaveTo = data.Save;
                            target.Title = "转换 " + target.From.Name;
                            target.BigTitle = "转换";
                            int pos = Source.IndexOf(target);
                            int counter = 0;
                            foreach (int i in ids)
                            {
                                counter++;
                                var title = (data.Esd ? "ESD" : "WIM")+"转换 " + target.From.Name + "里面的第" + i + "个镜像\n输出至:" + SaveTo;
                                if (counter == 1)
                                {
                                    target.Command = data.Esd ? new WimToEsdConvertCommand(target.From.FullName, i, SaveTo) : data.Wim ? new EsdToWimConvertCommand(target.From.FullName, i, SaveTo) : null;
                                    target.Title = title;
                                    target.Target = new FileInfo(SaveTo);
                                    target.BigTitle = "转换";
                                    target.InfoTitle = "转换为" + (data.Esd ? "ESD" : "WIM");
                                }
                                else
                                {
                                    Source.Insert(pos + 1, new TimeLineTaskItem()
                                    {
                                        From = target.From,
                                        Command = data.Esd ? new WimToEsdConvertCommand(target.From.FullName, i, SaveTo) : data.Wim ? new EsdToWimConvertCommand(target.From.FullName, i, SaveTo) : null,
                                        Title = title,
                                        Target = target.Target,
                                        Parent = target,
                                        BigTitle = "转换",
                                        InfoTitle = "转换为" + (data.Esd ? "ESD" : "WIM")
                                    });
                                }
                            }

                            //if (data.Wim)
                            //{
                            //}
                            //else if (data.Esd)
                            //{
                            //    target.Command = new WimToEsdCommand(target.From.FullName,, SaveTo);
                            //    target.Target = new FileInfo(SaveTo);
                            //}
                        }; break;
                    default:
                        break;
                }
                target.Update();
                String FileSizeMBStr(FileInfo f) => f.Exists ? (f.Length / 1024 / 1024) + "MB":"未知";
              
            };
        }
        private void ShowTopControl(FrameworkElement e) => GRID_Top.Children.Add(e);
        private void RemoveTopControl(FrameworkElement e) => GRID_Top.Children.Remove(e);
        private void TaskTimeLine_Drop(object sender, DragEventArgs e)
        {
            FileViewMode mode = e.Data.GetData(typeof(FileViewMode).Name) as FileViewMode;
            if (mode != null)
            {
                Source.Add(new TimeLineTaskItem() { From = mode.File, Title = "空动作", Target = Source.Count > 0 ? Source.Last().Target : null });
            }
        }
       
        public IEnumerable<TimeLineTaskItem> GetTimeLintTaskItems()
        {
            return Source;
        }

        private void MENU_ActionItem_Click(object sender, RoutedEventArgs e)
        {
            var tg = (sender as MenuItem).Tag as string;
            var task = (sender as MenuItem).DataContext as TimeLineTaskItem;
            switch (tg)
            {
                case "ActionLink":
                    {
                        var item = new TimeLineTaskItem() { From = task.Target, Parent = task };
                        Source.Insert(Source.IndexOf(task) + 1, item);
                    }; break;
                case "Delete":
                    {
                        List<TimeLineTaskItem> del = new List<TimeLineTaskItem>
                        {
                            task
                        };
                        foreach (var i in Source)
                        {
                            if (i.Parent == task||del.Any(a=>a==i.Parent)) del.Add(i);
                        }
                        foreach (var i in del)
                        {
                            Source.Remove(i);
                        }
                    }; break;
            }
        }

        private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) EditTimeLineItem((sender as ListBoxItem).DataContext as TimeLineTaskItem);
        }
    }
}
