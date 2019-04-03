using System;
using System.Runtime.InteropServices;
using Topshelf;

namespace ScreenLockDisable
{
    public class ScreenLocker : ServiceControl
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used.
            // ES_USER_PRESENT   = 0x00000004,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
        }

        private static void ScreenLockDisable()
        {
            if (SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                                        | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                                        | EXECUTION_STATE.ES_SYSTEM_REQUIRED
                                        | EXECUTION_STATE.ES_AWAYMODE_REQUIRED) == 0) //Away mode for Windows >= Vista
            {
                SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS
                                        | EXECUTION_STATE.ES_DISPLAY_REQUIRED
                                        | EXECUTION_STATE.ES_SYSTEM_REQUIRED); //Windows < Vista, forget away mode
            }
        }

        public bool Start(HostControl hostControl)
        {
            ScreenLockDisable();

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
