using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QDismSysISOManager.Utils.Model
{
    public class GetImageInfoCommand : DismCommand
    {
        public GetImageInfoCommand(string FileName)
        {
            this.SetArgs(FileName);
        }
        public override string Create()
        {
            return "/Get-ImageInfo /imagefile:"+Args[0];
        }
        public override string ToString()
        {
            return "获取Image 内容信息 :"+Args[0];
        }

    }
}
