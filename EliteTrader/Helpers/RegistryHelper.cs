using System;
using Microsoft.Win32;

namespace EliteTrader.Helpers
{
    public static class RegistryHelper
    {
        public static string GetVersion(Guid upgradeCode)
        {
            try
            {
                Guid? productCode = GetProductCode(upgradeCode);
                if (!productCode.HasValue)
                {
                    return null;
                }
                string registryKey = @"Installer\Products\" + productCode.Value.ReverseByteSequences().ToString("N").ToUpper();

                using (RegistryKey regKey = GetLocalMachineRegistryNode(registryKey))
                {
                    if (regKey != null)
                    {
                        uint dword = (uint)(int)regKey.GetValue("Version");

                        uint major = (dword & 0XFF000000) >> 24;
                        uint minor = (dword & 0X00FF0000) >> 16;
                        uint build = (dword & 0X0000FFFF);
                        return string.Format("{0}.{1}.{2}", major, minor, build);
                    }
                }
            }
            catch (Exception)
            {

            }

            return "<Unknown>";
        }

        private static Guid? GetProductCode(Guid upgradeCode)
        {
            string registryKey = @"Installer\UpgradeCodes\" + upgradeCode.ReverseByteSequences().ToString("N").ToUpper();
            try
            {
                using (RegistryKey regKey = GetLocalMachineRegistryNode(registryKey))
                {
                    if (regKey != null)
                    {
                        return new Guid(regKey.GetValueNames()[0]).ReverseByteSequences();
                    }
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        private static RegistryKey GetLocalMachineRegistryNode(string nodeName)
        {
            try
            {
                RegistryKey localMachine = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);

                RegistryKey rk = localMachine.OpenSubKey(nodeName);
                if (rk != null)
                {
                    return rk;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        //private static RegistryKey GetLocalMachineRegistryNode64Bit(string nodeName)
        //{
        //    if (!nodeName.StartsWith(@"SOFTWARE\Wow6432Node\"))
        //    {
        //        nodeName = nodeName.Replace(@"SOFTWARE\", @"SOFTWARE\Wow6432Node\");
        //    }
        //    return GetLocalMachineRegistryNode(nodeName);
        //}
    }
}
