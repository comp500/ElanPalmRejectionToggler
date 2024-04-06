using Microsoft.Win32;

namespace ElanPalmRejectionToggler
{
    internal static class StartupHandler
    {
        private const string ApplicationName = "ElanPalmRejectionToggler";

        public static bool IsRunningOnStartup()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
            return key?.GetValue(ApplicationName) as string == $"\"{Environment.ProcessPath}\"";
        }
        
        public static void SetRunOnStartup(bool runOnStartup)
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (runOnStartup)
            {
                key?.SetValue(ApplicationName, $"\"{Environment.ProcessPath}\"");
            }
            else
            {
                key?.DeleteValue(ApplicationName);
            }
        }
    }
}
