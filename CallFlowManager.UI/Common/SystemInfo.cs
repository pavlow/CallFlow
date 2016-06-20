using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Windows;

namespace CallFlowManager.UI.Common
{
    static class SystemInfo
    {
        const bool UseProcessorId = true;
        const bool UseBaseBoardProduct = true;
        const bool UseWindowsSerialNumber = true;        

        public static string GetSystemInfo(string pcId)
        {
            if(UseProcessorId)
                pcId += RunQuery("Processor", "ProcessorId");

            if (UseBaseBoardProduct)
                pcId += RunQuery("BaseBoard", "Product");

            if (UseWindowsSerialNumber)
                pcId += RunQuery("OperatingSystem", "SerialNumber");

            pcId = RemoveNonUse(pcId.ToUpper());
            
            if (pcId.Length < 30)
                return GetSystemInfo(pcId);

            return pcId;
        }

        private static string RemoveNonUse(string line)
        {
            for (var i = line.Length - 1; i >= 0; i--)
            {
                var ch = line[i];

                if ((ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9'))
                {
                    line = line.Remove(i, 1);
                }
            }
            return line;
        }

        private static string RunQuery(string device, string deviceParameter)
        {
            var objectSearcher = new ManagementObjectSearcher("Select * from Win32_" + device);
            foreach (var currentObject in objectSearcher.Get())
            {
                try
                {
                    return currentObject[deviceParameter].ToString();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return "";
        }
    }
}
