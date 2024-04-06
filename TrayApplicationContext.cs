using ElanPalmRejectionToggler.Properties;
using Microsoft.Win32;

namespace ElanPalmRejectionToggler
{
    internal class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon TrayIcon;
        private readonly ToolStripMenuItem RunOnBoot;
        private readonly ToolStripItem TogglePalmRejection;

        public TrayApplicationContext()
        {
            TrayIcon = new NotifyIcon()
            {
                Text = "Palm Rejection Enabled",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip(),
            };

            TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            TogglePalmRejection = TrayIcon.ContextMenuStrip.Items.Add("", null, TogglePalmRejection_Click);
            RunOnBoot = new ToolStripMenuItem("Run on Startup")
            {
                CheckOnClick = true
            };
            RunOnBoot.CheckedChanged += RunOnBoot_CheckedChanged;
            TrayIcon.ContextMenuStrip.Items.Add(RunOnBoot);
            TrayIcon.ContextMenuStrip.Items.Add("Quit", null, QuitMenuItem_Click);

            UpdateState();
        }

        private static bool IsDarkMode()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);
            if (key?.GetValue("AppsUseLightTheme") is int value)
            {
                return value != 1;
            }
            return false;
        }

        private void UpdateState()
        {
            bool palmRejectionEnabled = PalmRejectionHandler.IsPalmRejectionEnabled();
            bool darkMode = IsDarkMode();

            TogglePalmRejection.Text = palmRejectionEnabled ? "Disable Palm Rejection" : "Enable Palm Rejection";
            TrayIcon.Text = palmRejectionEnabled ? "Palm Rejection Enabled" : "Palm Rejection Disabled";
            if (palmRejectionEnabled)
            {
                TrayIcon.Icon = darkMode ? Resources.iconEnabledLight : Resources.iconEnabledDark;
            }
            else
            {
                TrayIcon.Icon = darkMode ? Resources.iconDisabledLight : Resources.iconDisabledDark;
            }
            RunOnBoot.Checked = StartupHandler.IsRunningOnStartup();
        }

        private void TrayIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            PalmRejectionHandler.SetPalmRejectionState(!PalmRejectionHandler.IsPalmRejectionEnabled());
            UpdateState();
        }

        private void TogglePalmRejection_Click(object? sender, EventArgs e)
        {
            PalmRejectionHandler.SetPalmRejectionState(!PalmRejectionHandler.IsPalmRejectionEnabled());
            UpdateState();
        }

        private void RunOnBoot_CheckedChanged(object? sender, EventArgs e)
        {
            StartupHandler.SetRunOnStartup(RunOnBoot.Checked);
            UpdateState();
        }

        private void QuitMenuItem_Click(object? sender, EventArgs e)
        {
            TrayIcon.Visible = false;
            Application.Exit();
        }
    }
}
