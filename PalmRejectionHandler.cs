using Microsoft.Win32;
using System.Diagnostics;

namespace ElanPalmRejectionToggler
{
    internal static class PalmRejectionHandler
    {
        public static bool IsPalmRejectionEnabled()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Elantech\SmartPad", false);
            if (key?.GetValue("DisableWhenType_Enable") is int value)
            {
                return value != 0;
            }
            return true;
        }

        private static void SetPalmRejectionKey(bool enable)
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Elantech\SmartPad", true);
            key?.SetValue("DisableWhenType_Enable", enable ? 1 : 0, RegistryValueKind.DWord);
            key?.SetValue("DisableWhenType_AllArea", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        private class ProcessStartException: Exception;

        public static void SetPalmRejectionState(bool enabled)
        {
            bool prevState = IsPalmRejectionEnabled();
            try
            {
                SetPalmRejectionKey(enabled);

                ProcessStartInfo startInfo = new()
                {
                    UseShellExecute = true,
                    FileName = Environment.ProcessPath,
                    Verb = "runas",
                    Arguments = "restartDevice"
                };

                using Process proc = Process.Start(startInfo) ?? throw new ProcessStartException();
                proc.WaitForExit();
                if (proc.ExitCode != 100)
                {
                    throw new ProcessStartException();
                }
            }
            catch (Exception)
            {
                try
                {
                    SetPalmRejectionKey(prevState);
                }
                catch (Exception) { }
            }
        }

        private static void RunPnpUtil(string args)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "pnputil",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process proc = Process.Start(startInfo) ?? throw new ProcessStartException();
            var output = proc.StandardOutput.ReadToEnd();
            var error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new ProcessStartException();
            }
        }

        public static void RestartDevice()
        {
            RunPnpUtil(@"/disable-device HID\ELAN0626&Col01\5&43f07bd&1&0000");
            RunPnpUtil(@"/enable-device HID\ELAN0626&Col01\5&43f07bd&1&0000");
        }
    }
}
