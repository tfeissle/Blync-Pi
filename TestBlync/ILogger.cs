using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBlync
{
    public interface ILogger
    {
        void LogInfo(string message);

        void LogInfo(string format, params object[] args);

        void LogWarning(string message);

        void LogWarning(string format, params object[] args);

        void LogError(string message);

        void LogError(string format, params object[] args);
    }
}
