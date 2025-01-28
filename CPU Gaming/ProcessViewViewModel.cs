using System.ComponentModel;
using System.Diagnostics;

namespace CPU_Gaming
{
    class ProcessViewViewModel : INotifyPropertyChanged
    {
        private int _pid = 0;
        private string _windowName = "";
        private string _processName = "";
        private string _execatubleName = "";        
        private string _affinityFlag = "";
        private string _priorityClass = "";

        public int PID
        {
            get => _pid;
            set
            {
                _pid = value;
                OnPropertyChanged(nameof(PID));
            }
        }

        public string WindowName
        {
            get => _windowName;
            set
            {
                _windowName = value;
                OnPropertyChanged(nameof(WindowName));
            }
        }

        public string ProcessName
        {
            get => _processName;
            set
            {
                _processName = value;
                OnPropertyChanged(nameof(ProcessName));
            }
        }

        public string ExecatubleName
        {
            get => _execatubleName;
            set
            {
                _execatubleName = value;
                OnPropertyChanged(nameof(ExecatubleName));
            }
        }

        public string ProcessAffinity
        {
            get => _affinityFlag;
            set
            {
                _affinityFlag = value;
                OnPropertyChanged(nameof(ProcessAffinity));
            }
        }

        public string ProcessPriority
        {
            get => _priorityClass;
            set
            {
                _priorityClass = value;
                OnPropertyChanged(nameof(ProcessPriority));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
