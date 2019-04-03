using System;
using System.Reflection;
using Topshelf;

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

        static void Main(string[] args)
        {
            var host = HostFactory.New(x =>
            {
                x.Service<ScreenLocker>(sc =>
                {
                    sc.ConstructUsing(() => new ScreenLocker());

                    // the start and stop methods for the service
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));

                    // optional, when shutdown is supported
                    sc.WhenShutdown((s, hostControl) => s.Shutdown(hostControl));
                });

                x.RunAsLocalSystem();
                x.SetServiceName("ScreenLockDisable_Service");
                x.SetDisplayName("Screen Lock Disable");
                x.SetDescription("Screen Lock Disable service application.");
                x.EnableShutdown();
                x.StartAutomatically();
            });

            var serviceExitCode = host.Run();

            Environment.ExitCode = (int)serviceExitCode;
        }
    }
}
