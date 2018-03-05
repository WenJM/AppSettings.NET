using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettings.Client.Exception
{
    public class AppSettingException : System.Exception
    {
        public AppSettingException(string message) : base(message) { }
    }
}
