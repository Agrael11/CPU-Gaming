using System.Diagnostics;
using System.Management;

namespace CPU_Gaming
{
    internal static class WMIScheduler
    {
        private static ManagementEventWatcher? watcher;
        public static void RegisterWatcher(Action<CPUProcessInfo> newProcessSpawnedCallback)
        {
            watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            watcher.EventArrived += (sender, e) =>
            {
                var newEvent = e.NewEvent;
                var id = (uint)newEvent.Properties["ProcessID"].Value;
                var info = new CPUProcessInfo(Process.GetProcessById((int)id));
                newProcessSpawnedCallback(info);
            };
            watcher.Start();
        }

        public static void UnregisterWatcher()
        {
            watcher?.Stop();
        }
    }
}
