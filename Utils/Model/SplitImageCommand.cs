using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDismSysISOManager.Utils.Model
{
    public class SplitImageCommand : DismCommand
    {
        /// <param name="SourceFile">源文件</param>
        /// <param name="SaveTo">保存至</param>
        /// <param name="SplitSizeMB">每个大小</param>
        public SplitImageCommand(string SourceFile, String SaveTo, long SplitSizeMB)
        {
            SetArgs(SourceFile, SaveTo, SplitSizeMB);
        }
        public override string Create()
        {
            return "/Split-Image /ImageFile:" + Args[0] + " /SWMFile:" + Args[1] + " /FileSize:" + Args[2];
        }
        public override string ToString()
        {
            return "切割Image :源文件"+Args[0]+"->切割至"+Args[1]+" 理想尺寸"+Args[2];
        }
    }
}
