using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDismSysISOManager.Utils.Model
{
    public interface IDismCommand
    {
        public bool SetArgs(params object[] Args);
        public String Create();
    }
}
