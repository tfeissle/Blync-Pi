using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBlync
{
    public interface IConfigurationProvider
    {
        string GetConfigurationSettingValue(string configurationSettingName);
        string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue);
    }
}
