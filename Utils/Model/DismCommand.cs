using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDismSysISOManager.Utils.Model
{
    public abstract class DismCommand : IDismCommand
    {
        private String _Command = null;
        protected object[] Args = new string[0];
        public string Command { get => (_Command ?? (_Command = Create())); }
        public abstract string Create();

        public virtual bool SetArgs(params object[] Args)
        {
            this.Args = Args;
            _Command = null;
            return true;
        }
    }
}
