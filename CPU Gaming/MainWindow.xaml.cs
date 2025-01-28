using System.Diagnostics;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace CPU_Gaming
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private bool CloseMe = false;

        List<ProcessSetting> settingsList = [];

        CancellationTokenSource source = new();

        NotifyIcon icon;

        public MainWindow()
        {
            InitializeComponent();
            icon = new NotifyIcon();
            icon.Icon = new Icon("Icon.ico");
            icon.Text = "CPU Gaming";
            icon.Visible = false;
            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.ContextMenuStrip.Items.Add("Show Window", null, (sender, e) => { icon.Visible = false; Visibility = Visibility.Visible; });
            icon.ContextMenuStrip.Items.Add("Exit", null, (sender, e) => { CloseMe = true; Close(); });
        }

        public async Task SearchForActive(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var processes = CPUProcessInfo.GetCPUProcesses(true).ToList();
                foreach (var process in processes.Where(p => settingsList.Any(s => s.ExecatubleTarget == p.ProcessExecatubleName)))
                {
                    foreach (var setting in settingsList.Where(s => s.ExecatubleTarget == process.ProcessExecatubleName))
                    {
                        process.Apply(setting);
                    }
                }
                await Task.Delay(1000, token);
            }
        }

        public static void ForceCheck(ProcessSetting setting)
        {
            var processes = CPUProcessInfo.GetCPUProcesses(true).ToList();
            foreach (var process in processes.Where(p => setting.ExecatubleTarget == p.ProcessExecatubleName))
            {
                process.Apply(setting);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            source = new CancellationTokenSource();

            var settings = ProcessSetting.Load("settings.json");
            if (settings != null)
            {
                settingsList = settings;
                foreach (var setting in settingsList)
                {
                    var processView = new ProcessView(setting, -1, setting.RuleName, setting.LastWindowName, setting.ExecatubleTarget,
                        setting.Priority, setting.AffinityFlag, RemoveFromConfigured, MoveToConfigured);
                    processView.EnableReset();
                    configuredProcesses.Items.Add(processView);
                }
            }

            UpdateProcesses();
            _ = SearchForActive(source.Token);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateProcesses();
        }

        private void MoveToConfigured(ProcessView view)
        {
            availableProcesses.Items.Remove(view);
            settingsList.Add(view.setting);
            ProcessSetting.Save("settings.json", settingsList);
            configuredProcesses.Items.Add(view);
        }

        private void RemoveFromConfigured(ProcessView view)
        {
            configuredProcesses.Items.Remove(view);
            settingsList.Remove(view.setting);
            ProcessSetting.Save("settings.json", settingsList);
            UpdateProcesses();
        }

        private void UpdateProcesses()
        {
            availableProcesses.Items.Clear();

            var processes = CPUProcessInfo.GetCPUProcesses(true);
            foreach (var process in processes)
            {
                if (settingsList.Any(p => p.ExecatubleTarget == process.ProcessExecatubleName))
                    continue;

                var processSetting = new ProcessSetting(process.ProcessName, process.ProcessExecatubleName, process.WindowTitle, process.Affinity, process.PriorityClass);

                var processView = new ProcessView(processSetting, process.ProcessID, process.ProcessName, process.WindowTitle, process.ProcessExecatubleName,
                    process.PriorityClass, process.Affinity, RemoveFromConfigured, MoveToConfigured);

                availableProcesses.Items.Add(processView);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateProcesses();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            source.Cancel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseMe)
            {
                return;
            }
            Hide();
            icon.Visible = true;
            e.Cancel = true;
        }
    }
}