using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ScreenLockDisable
{
    class Program
    {
        #region Program configuration

        const string VERSION_INFO = "";

        #if DEBUG
        const string BUILD_CONFIGURATION = " [Debug]";
        #else
        const string BUILD_CONFIGURATION = "";
        #endif

        #endregion

        #region Build/program info

        private static Version Ver { get; } = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
        public static string ProgramVersion { get; } = $"{Ver.Major}.{Ver.Minor}.{Ver.Build}";
        public static string ProgramBaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string ProgramPath { get; } = Assembly.GetEntryAssembly().Location;
        //public static string ProgramSHA256 { get; } = Utils.CalculateFileSHA256(ProgramPath);
        public static string ProgramName { get; } = Assembly.GetExecutingAssembly().GetName().Name;
        public static string ProgramHeader { get; } = $"{ProgramName} v{ProgramVersion}{VERSION_INFO}{BUILD_CONFIGURATION}";
        //public static string ProgramBuild { get; } = $"Build: {Properties.Resources.BuildTime} (UTC) CLR: {Assembly.GetExecutingAssembly().ImageRuntimeVersion} SHA256: {ProgramSHA256}";
        //public static string ProgramLastCommit { get; } = $"Commit: {Properties.Resources.LastCommit}";
        public static string ProgramAuthor { get; } = "Author: Daniil Shipilin";

        #endregion

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        const uint SW_SHOWMINIMIZED = 0x00000002;

        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);

        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        //const uint ES_USER_PRESENT    = 0x00000004, // legacy flag
        const uint ES_AWAYMODE_REQUIRED = 0x00000040;
        const uint ES_CONTINUOUS = 0x80000000;

        static void Main(string[] args)
        {
            // launch console window minimized
            ShowWindow(Process.GetCurrentProcess().MainWindowHandle, SW_SHOWMINIMIZED);
            Console.Title = ProgramHeader;
            Console.CursorVisible = false;
            Console.WriteLine("Press 'Q' key to exit");

            var task = ScreenLockDisableAsync();

            while (Console.ReadKey(true).Key != ConsoleKey.Q) { continue; }
        }

        private static async Task ScreenLockDisableAsync()
        {
            // execute forever (this task/thread should always be alive)
            while (true)
            {
                // away mode for Windows >= Vista
                uint res = SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED | ES_AWAYMODE_REQUIRED);

                if (res == 0)
                {
                    // Windows < Vista, forget away mode
                    SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
                }

                await Task.Delay(60000);
            }
        }
    }
}
