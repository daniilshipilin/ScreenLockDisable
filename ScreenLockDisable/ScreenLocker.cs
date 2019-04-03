using System.Runtime.InteropServices;
using Topshelf;

namespace ScreenLockDisable
{
    public class ScreenLocker : ServiceControl
    {
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);

        const uint ES_SYSTEM_REQUIRED   = 0x00000001;
        const uint ES_DISPLAY_REQUIRED  = 0x00000002;
        //const uint ES_USER_PRESENT    = 0x00000004, // legacy flag
        const uint ES_AWAYMODE_REQUIRED = 0x00000040;
        const uint ES_CONTINUOUS        = 0x80000000;

        public bool Start(HostControl hostControl)
        {
            // away mode for Windows >= Vista
            uint res = SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED | ES_AWAYMODE_REQUIRED);

            if (res == 0)
            {
                // Windows < Vista, forget away mode
                SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
            }

            return (true);
        }

        public bool Stop(HostControl hostControl)
        {
            return (true);
        }

        public bool Shutdown(HostControl hostControl)
        {
            return (true);
        }
    }
}
