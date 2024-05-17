using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDismSysISOManager.Utils.Model
{
    public class EsdToWimConvertCommand : DismCommand
    {
        public EsdToWimConvertCommand(string SourceFilePath,int SourceIndex, string ConvertToFilePath)
        {
            this.SetArgs(SourceFilePath, SourceIndex, ConvertToFilePath);    
        }
        public override string Create()
        {
            return "/Export-Image /SourceImageFile:" + Args[0] +" /SourceIndex:" + Args[1] +" /DestinationImageFile:" + Args[2] +" /Compress:recovery";
        }
    }
}
