using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteTrader.EliteOcr.Logging
{
    public enum EnumMessageType
    {
        Error = 5,
        Warning = 10,
        Info = 20,
        Debug = 30
    }

    public interface ILogger
    {
        void Log(string message, EnumMessageType messageType = EnumMessageType.Info);
    }
}
