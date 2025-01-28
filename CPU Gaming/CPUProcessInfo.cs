using System.Diagnostics;

namespace CPU_Gaming
{
    internal class CPUProcessInfo
    {
        private (int state, string name) _cachedWindowTitle = (0, "");
        private readonly Process _processHandle;

        public string ProcessName { get; private set; }
        public string ProcessExecatubleName { get; private set; }
        public string WindowTitle { get { return GetProcessTitle(); } }

        public ulong Affinity { get { return (ulong)_processHandle.ProcessorAffinity; } set { _processHandle.ProcessorAffinity = (nint)value; } }
        public ProcessPriorityClass PriorityClass { get { return _processHandle.PriorityClass; } set { _processHandle.PriorityClass = value; } }
        public int ProcessID { get; private set; }

        public void Apply(ProcessSetting setting)
        {
            ProcessSetting.SetForProcess(setting, _processHandle);
        }

        public CPUProcessInfo(Process processHandle)
        {
            _processHandle = processHandle;

            ProcessID = _processHandle.Id;
            ProcessName = _processHandle.ProcessName;
            if (_processHandle.MainModule is null) throw new Exception("This module has no available main module");
            ProcessExecatubleName = _processHandle.MainModule.FileName;
        }
        public CPUProcessInfo(Process processHandle, string windowName)
        {
            _processHandle = processHandle;

            ProcessID = _processHandle.Id;
            ProcessName = _processHandle.ProcessName;
            if (_processHandle.MainModule is null) throw new Exception("This module has no available main module");
            ProcessExecatubleName = _processHandle.MainModule.FileName;
            _cachedWindowTitle = (100, windowName);
        }

        public Process GetProcessHandle()
        {
            return _processHandle;
        }


        private string GetProcessTitle()
        {
            _cachedWindowTitle.state--;

            if (_cachedWindowTitle.state <= 0)
            {
                if (_processHandle.TryGetProcessWindow(out var title))
                {
                    _cachedWindowTitle.name = title;
                    _cachedWindowTitle.state = 100;
                }
            }

            return _cachedWindowTitle.name;
        }

        public static IEnumerable<CPUProcessInfo> GetCPUProcesses(bool visibleOnly)
        {
            var processes = (IEnumerable<Process>)Process.GetProcesses();
            if (visibleOnly)
            {
                processes = processes.Where(p => p.MainWindowHandle != IntPtr.Zero);
            }

            foreach (var process in processes)
            {
                CPUProcessInfo? cpuInfo =  null;
                try
                {
                    var cpu = new CPUProcessInfo(process);
                    cpuInfo = cpu;
                }
                catch
                {
                }
                if (cpuInfo is null) continue;
                yield return cpuInfo;
            }
        }
    }
}
