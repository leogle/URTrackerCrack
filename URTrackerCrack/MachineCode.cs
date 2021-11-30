using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Text;

namespace URTrackerCrack
{
    public static class MachineCode
    {
        private static string GetProcessId()
        {
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();
                string str = (string)null;
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        str = enumerator.Current.Properties["ProcessorId"].Value.ToString();
                }
                return str;
            }
            catch
            {
                return "FFFF";
            }
        }

        private static string GetDiskNum()
        {
            try
            {
                ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
                managementObject.Get();
                return managementObject.GetPropertyValue("VolumeSerialNumber").ToString();
            }
            catch
            {
                return "FFFF";
            }
        }

        public static string GetMachineCode()
        {
            if (ConfigurationManager.AppSettings["LOCAL_MC"] == "1")
                return "FFFFFFFF";
            string str1 = MachineCode.GetProcessId();
            string str2 = MachineCode.GetDiskNum();
            return str1.Substring(str1.Length - 4) + str2.Substring(str2.Length - 4);
        }
    }
}
