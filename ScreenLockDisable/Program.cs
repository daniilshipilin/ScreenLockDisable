namespace ScreenLockDisable;

using System.Reflection;
using System.Runtime.InteropServices;

internal class Program
{
#if DEBUG
    private const string BUILD_CONFIGURATION = " [Debug]";
#else
    private const string BUILD_CONFIGURATION = "";
#endif

    private static Version Ver { get; } = new AssemblyName(Assembly.GetExecutingAssembly().FullName!).Version!;

    private static string ProgramVersion { get; } = $"{Ver.Major}.{Ver.Minor}.{Ver.Build}";

    private static string ProgramName { get; } = Assembly.GetExecutingAssembly().GetName().Name!;

    private static string ProgramHeader { get; } = $"{ProgramName} v{ProgramVersion}{BUILD_CONFIGURATION}";

    [DllImport("kernel32.dll")]
    static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    [Flags]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040, // This flag must be combined with ES_CONTINUOUS. If the machine is configured to allow it, this indicates that the thread requires away mode. When in away mode the computer will appear to sleep as normal. However, the thread will continue to execute even though the computer has partially suspended. As this flag gives the false impression that the computer is in a low power state, you should only use it when absolutely necessary.
        ES_CONTINUOUS = 0x80000000, // This flag is used to specify that the behaviour of the two previous flags is continuous. Rather than resetting the idle timers once, they are disabled until you specify otherwise. Using this flag means that you do not need to call SetThreadExecutionState repeatedly.
        ES_DISPLAY_REQUIRED = 0x00000002, // This flag indicates that the display is in use. When passed by itself, the display idle timer is reset to zero once. The timer restarts and the screensaver will be displayed when it next expires.
        ES_SYSTEM_REQUIRED = 0x00000001  // This flag indicates that the system is active. When passed alone, the system idle timer is reset to zero once. The timer restarts and the machine will sleep when it expires.
    }

    internal static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0].Equals("/?"))
            {
                Console.WriteLine(ProgramHeader);

                return;
            }
        }

        Console.Title = ProgramHeader;
        Console.CursorVisible = false;
        Console.WriteLine("'Q': key to exit");
        Console.WriteLine("'D': key to disable screen lock");
        Console.WriteLine("'E': key to enable screen lock");

        ScreenLockDisable();

        while (true)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Q:
                    return;

                case ConsoleKey.E:
                    ScreenLockEnable();
                    break;

                case ConsoleKey.D:
                    ScreenLockDisable();
                    break;

                default:
                    Console.WriteLine("Unknown key press detected");
                    break;
            }
        }
    }

    private static void ScreenLockDisable()
    {
        // To disable it until we state otherwise, we use the ES_DISPLAY_REQUIRED and ES_CONTINUOUS flags.
        SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        Console.WriteLine("Screen lock disabled");
    }

    private static void ScreenLockEnable()
    {
        // Re-enabling the screensaver requires that we clear the ES_DISPLAY_REQUIRED state flag. We can do this by passing the ES_CONTINUOUS flag alone
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        Console.WriteLine("Screen lock enabled");
    }
}
