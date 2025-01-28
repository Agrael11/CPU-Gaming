using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace CPU_Gaming
{
    public class ProcessSetting(string ruleName, string lastWindowName, string execatubleTarget, ulong affinityFlag, ProcessPriorityClass priority)
    {
        public string RuleName { get; set; } = ruleName;
        public string LastWindowName { get; set; } = lastWindowName;
        public string ExecatubleTarget { get; set; } = execatubleTarget;
        public ulong AffinityFlag { get; set; } = affinityFlag;
        public ProcessPriorityClass Priority { get; set; } = priority;

        public static void SetForProcess(ProcessSetting setting, Process process)
        {
            process.ProcessorAffinity = (nint)setting.AffinityFlag;
            process.PriorityClass = setting.Priority;
        }

        public static List<ProcessSetting>? Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            var text = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<ProcessSetting>>(text);
        }

        public static void Save(string path, List<ProcessSetting> settings)
        {
            var data = JsonSerializer.Serialize(settings);
            if (data is not null)
            {
                File.WriteAllText(path, data);
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RuleName, ExecatubleTarget, LastWindowName, AffinityFlag, Priority);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ProcessSetting other) return false;

            return other.RuleName == RuleName && other.LastWindowName == LastWindowName && other.ExecatubleTarget == ExecatubleTarget && other.AffinityFlag == AffinityFlag && other.Priority == Priority;
        }
    }
}
