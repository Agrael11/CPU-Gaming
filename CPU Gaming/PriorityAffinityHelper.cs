using System.Diagnostics;
using System.Windows.Documents;

namespace CPU_Gaming
{
    public static class PriorityAffinityHelper
    {
        public static string AffinityString(ulong affinity)
        {
            int core = 0;
            var coresEnabled = new List<int>();
            while (affinity > 0)
            {
                if (affinity % 2 == 1)
                {
                    coresEnabled.Add(core);
                }
                affinity /= 2;
                core++;
            }
            return string.Join(", ", coresEnabled);
        }

        public static string PriorityString(ProcessPriorityClass priorityClass)
        {
            return priorityClass switch
            {
                ProcessPriorityClass.Normal => "Normal",
                ProcessPriorityClass.Idle => "Low",
                ProcessPriorityClass.High => "High",
                ProcessPriorityClass.RealTime => "Realtime",
                ProcessPriorityClass.BelowNormal => "Below Normal",
                ProcessPriorityClass.AboveNormal => "Above Normal",
                _ => "Unknown",
            };
        }

        public static int GetCPUCount()
        {
            return Environment.ProcessorCount;
        }

        public static ulong EnableCores(ulong affinity, params int[] cores)
        {
            foreach (var core in cores)
            {
                affinity |= 1UL << core;
            }
            return affinity;
        }

        public static ulong DisableCores(ulong affinity, params int[] cores)
        {
            var temp = ulong.MaxValue;
            foreach (var core in cores)
            {
                temp ^= 1UL << core;
            }
            return affinity & temp;
        }

        public static List<int> GetEnabledCores(ulong affinity)
        {
            var cores = new List<int>();
            var core = 0;
            while (affinity > 0)
            {
                if (affinity % 2 == 1) cores.Add(core);
                core++;
                affinity /= 2;
            }
            return cores;
        }

        public static bool IsCoreEnabled(ulong affinity, int core)
        {
            return ((1UL << core) & affinity) != 0;
        }
    }
}