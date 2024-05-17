using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDismSysISOManager.Utils.Model
{
    public class ExportAppendImageCommand : DismCommand
    {
        public ExportAppendImageCommand(string sourceFilePath,int idx,string saveTo) { 
            this.SetArgs(sourceFilePath,idx,saveTo);    
        }
        public override string Create()
        {
            return "/Export-Image /SourceImageFile:" + Args[0] +" /SourceIndex:" + Args[1] +" /DestinationImageFile:"+Args[2];
        }
    }
}
