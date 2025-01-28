using System.Configuration;
using System.Diagnostics;
using System.IO.Pipes;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace CPU_Gaming
{
    /// <summary>
    /// Interaction logic for ProcessView.xaml
    /// </summary>
    public partial class ProcessView : UserControl
    {
        private readonly ProcessViewViewModel viewModel;
        public ProcessSetting setting = new("", "", "", 0, ProcessPriorityClass.Idle);
        private int CPUS = 0;
        private readonly List<MenuItem> affinityMenus = [];

        public int PID { get => viewModel.PID; set { viewModel.PID = value; } }
        public string ProcessName { get => setting.RuleName; set { setting.RuleName = value; viewModel.ProcessName = value; } }
        public string WindowName { get => setting.LastWindowName; set { setting.LastWindowName = value; viewModel.WindowName = value; } }
        public string ExecatubleName { get => setting.ExecatubleTarget; set { setting.ExecatubleTarget = value; viewModel.ExecatubleName = value; } }
        public ProcessPriorityClass ProcessPriority { get => setting.Priority; set { setting.Priority = value; viewModel.ProcessPriority = PriorityAffinityHelper.PriorityString(value); } }
        public ulong ProcessAffinity { get => setting.AffinityFlag; set { setting.AffinityFlag = value; viewModel.ProcessAffinity = PriorityAffinityHelper.AffinityString(value); } }

        public bool Configured = false;
        public Action<ProcessView>? RemoveConfigured;
        public Action<ProcessView>? MoveToConfigured;

        public ProcessView()
        {
            InitializeComponent();
            viewModel = new ProcessViewViewModel();
            Setup();
        }
        
        public void Setup()
        {
            DataContext = viewModel;
            for (var core = 0; core < PriorityAffinityHelper.GetCPUCount(); core++)
            {
                var item = new MenuItem
                {
                    Header = "CPU Core #" + core,
                    Tag = core,
                    IsCheckable = true
                };
                if (PriorityAffinityHelper.IsCoreEnabled(setting.AffinityFlag, core))
                {
                    item.IsChecked = true;
                    CPUS++;
                }
                item.Click += Affinity_Checked;
                item.StaysOpenOnClick = true;
                affinityMenus.Add(item);
                Affinity.Items.Add(item);
            }
            AllAffinity.IsChecked = CPUS == PriorityAffinityHelper.GetCPUCount();

            switch (setting.Priority)
            {
                case ProcessPriorityClass.Normal:
                    NormalMenu.IsChecked = true;
                    break;
                case ProcessPriorityClass.Idle:
                    IdleMenu.IsChecked = true;
                    break;
                case ProcessPriorityClass.High:
                    HighMenu.IsChecked = true;
                    break;
                case ProcessPriorityClass.RealTime:
                    RealtimeMenu.IsChecked = true;
                    break;
                case ProcessPriorityClass.BelowNormal:
                    LowMenu.IsChecked = true;
                    break;
                case ProcessPriorityClass.AboveNormal:
                    AboveMenu.IsChecked = true;
                    break;
            }
        }

        public ProcessView(ProcessSetting setting, int pID, string processName, string windowName, string execatubleName, ProcessPriorityClass processPriority, ulong processAffinity, Action<ProcessView>? removeConfigured, Action<ProcessView>? moveToConfigured)
        {
            InitializeComponent();
            viewModel = new ProcessViewViewModel();
            
            this.setting = setting;
            PID = pID;
            ProcessName = processName;
            WindowName = windowName;
            ExecatubleName = execatubleName;
            ProcessPriority = processPriority;
            ProcessAffinity = processAffinity;
            RemoveConfigured = removeConfigured;
            MoveToConfigured = moveToConfigured;

            Setup();
        }

        private void Affinity_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                if (item.Tag.GetType() == typeof(string) && (string)item.Tag == "AllProcesses")
                {
                    if (CPUS == PriorityAffinityHelper.GetCPUCount())
                    {
                        CPUS = 1;
                        ProcessAffinity = 1;
                        affinityMenus.ForEach((menuItem) => { menuItem.IsChecked = false; });
                        affinityMenus[0].IsChecked = true;
                    }
                    else
                    {
                        CPUS = PriorityAffinityHelper.GetCPUCount();
                        ProcessAffinity = (ulong)((1 << CPUS) - 1);
                        affinityMenus.ForEach((menuItem) => { menuItem.IsChecked = true; });
                    }

                    if (!Configured)
                    {
                        MoveToConfigured?.Invoke(this);
                        EnableReset();
                    }
                    AllAffinity.IsChecked = CPUS == PriorityAffinityHelper.GetCPUCount();
                    return;
                }

                if (CPUS == 1 && !item.IsChecked) return;

                if (item.IsChecked) ProcessAffinity = PriorityAffinityHelper.EnableCores(setting.AffinityFlag, (int)item.Tag);
                else ProcessAffinity = PriorityAffinityHelper.DisableCores(setting.AffinityFlag, (int)item.Tag);
                
                if (!Configured)
                {
                    MoveToConfigured?.Invoke(this);
                    EnableReset();
                }
                AllAffinity.IsChecked = CPUS == PriorityAffinityHelper.GetCPUCount();
                return;
            }
        }

        private void Priority_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                if (item.IsChecked) return;
                NormalMenu.IsChecked = false;
                LowMenu.IsChecked = false;
                IdleMenu.IsChecked = false;
                AboveMenu.IsChecked = false;
                HighMenu.IsChecked = false;
                RealtimeMenu.IsChecked = false;
                
                item.IsChecked = true;

                ProcessPriority = item.Tag switch
                {
                    "0" => ProcessPriorityClass.Idle,
                    "1" => ProcessPriorityClass.BelowNormal,
                    "3" => ProcessPriorityClass.AboveNormal,
                    "4" => ProcessPriorityClass.High,
                    "5" => ProcessPriorityClass.RealTime,
                    _ => ProcessPriorityClass.Normal,
                };
                if (!Configured)
                {
                    MoveToConfigured?.Invoke(this);
                    EnableReset();
                }
            }
        }

        public void EnableReset()
        {
            Configured = true;
            ResetButton.Visibility = System.Windows.Visibility.Visible;
        }

        public void DisableReset()
        {
            Configured = false;
            ResetButton.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ResetMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.ForceCheck(setting);
            RemoveConfigured?.Invoke(this);
            DisableReset();
        }
    }
}
