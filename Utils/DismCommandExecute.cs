using QDismSysISOManager.Utils.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QDismSysISOManager.Utils
{
    public class DismCommandExecute : IDisposable
    {
        public event Action<DismCommandExecute, string> Output;
        public bool IsRunning { get; protected set; }

        public event Action<DismCommandExecute> Disposeds;
        private StringBuilder TempOutput = new StringBuilder();
        private DateTime LastOutputTime = DateTime.MinValue;
        public DismCommandExecute()
        {
            Disposeds += DismCommandExecute_Disposeds;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (TempOutput.Length > 0 && (DateTime.Now - LastOutputTime).TotalSeconds > 0.5)
            {
                Output?.Invoke(this, TempOutput.ToString());
                TempOutput.Clear();
                LastOutputTime = DateTime.Now;
            }
        }

        private void DismCommandExecute_Disposeds(DismCommandExecute obj)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        public void WriteCommand(DismCommand cmd)
        {
            IsRunning = true;
            var task = CreateCMD(cmd);
            task.WaitForExit();
            IsRunning = false;
        }
        public void WriteCommandNoWait(DismCommand cmd)
        {
            var task = CreateCMD(cmd);
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                task.Close();
            });
        }
        //public Task<string> WriteCommandTask(DismCommand cmd)
        //{
        //    return Task.Run(() =>
        //    {
        //        IsRunning = true;
        //        var task = CreateCMD(cmd);
        //        task.WaitForExit();
        //        IsRunning = false;
        //        return task.StandardOutput.ReadToEnd();
        //    });
        //}
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Output?.Invoke(this, e.Data);
            //if(e.Data!=null)
            this.TempOutput.AppendLine(e.Data);
        }
        private void Process_Exited(object? sender, EventArgs e)
        {

        }
        protected Process CreateCMD(DismCommand cmd)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            Process process = new Process();
            startInfo.FileName = "Dism.exe"; // 指定要运行的命令或可执行文件  
            startInfo.Arguments = cmd.Command;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true; // 如果不需要显示CMD窗口，则设置为true  
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.Exited += Process_Exited;
            process.Start();
            process.BeginOutputReadLine();
            Disposeds += (ss) => process.Kill();
            return process;
        }

        public void Dispose()
        {
            Disposeds?.Invoke(this);
        }
    }
}
