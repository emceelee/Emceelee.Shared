using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Emceelee.Shared
{
    public class PerformanceLogger
    {
        private Dictionary<string, PerformanceLog> _performanceLogs = new Dictionary<string, PerformanceLog>();
        private Stopwatch _stopwatch = new Stopwatch();

        public void Clear()
        {
            _performanceLogs.Clear();
            _stopwatch.Reset();
        }

        private PerformanceLog GetPerformanceLog(string key)
        {
            PerformanceLog p = null;
            if (!_performanceLogs.TryGetValue(key, out p))
            {
                p = new PerformanceLog();
                _performanceLogs.Add(key, p);
            }

            return p;
        }

        public void Start(string key)
        {
            PerformanceLog p = GetPerformanceLog(key);
            p.Start();
        }

        public void Stop(string key)
        {
            PerformanceLog p = GetPerformanceLog(key);
            p.Stop();
        }

        public TimeSpan Elapsed(string key)
        {
            PerformanceLog p = GetPerformanceLog(key);

            return p.Elapsed;
        }

        public void Start() { _stopwatch.Start(); }

        public void Stop() { _stopwatch.Stop(); }

        public TimeSpan Elapsed() { return _stopwatch.Elapsed; }

        public Dictionary<string, Metrics> CalculateMetrics()
        {
            var result = new Dictionary<string, Metrics>();

            foreach(var pair in _performanceLogs)
            {
                var m = pair.Value.CalculateMetrics(Elapsed());
                result.Add(pair.Key, m);
            }

            return result;
        }

        public string Results()
        {
            var result = CalculateMetrics();
            var sb = new StringBuilder();

            foreach(var pair in result)
            {
                sb.AppendLine("{" + pair.Key + "} - " + pair.Value.PercentTotal.ToString() + "%");
                sb.AppendLine("\tCount - " + pair.Value.Count.ToString());
                sb.AppendLine("\tTotalElapsed - " + pair.Value.TotalElapsed.ToString());
                sb.AppendLine("\tMinElapsed - " + pair.Value.MinElapsed.ToString());
                sb.AppendLine("\tMaxElapsed - " + pair.Value.MaxElapsed.ToString());
                sb.AppendLine("\tAverageElapsed - " + pair.Value.AverageElapsed.ToString());
            }
            sb.AppendLine("{ TOTAL } - " + Elapsed().ToString());
            sb.AppendLine();

            return sb.ToString();
        }
    }

    public class PerformanceLog
    {
        public List<TimeSpan> RunTimes = new List<TimeSpan>();
        public TimeSpan Elapsed
        {
            get
            {
                TimeSpan total;
                foreach(var ts in RunTimes)
                    total += ts;
                
                if(Stopwatch.IsRunning)
                    total += Stopwatch.Elapsed;

                return total;
            }
        }

        private Stopwatch Stopwatch = new Stopwatch();

        public void Start()
        {
            Stopwatch.Start();
        }

        public void Stop()
        {
            Stopwatch.Stop();
            RunTimes.Add(Stopwatch.Elapsed);
            Stopwatch.Reset();
        }

        public Metrics CalculateMetrics(TimeSpan totalElapsed)
        {
            var m = new Metrics();
            m.Count = RunTimes.Count;

            if(m.Count > 0)
            {
                var elapsed = Elapsed;
                m.TotalElapsed = elapsed;
                m.MinElapsed = RunTimes.Min();
                m.MaxElapsed = RunTimes.Max();
                m.AverageElapsed = new TimeSpan(elapsed.Ticks / m.Count);

                if(totalElapsed.Ticks > 0)
                {
                    m.PercentTotal = (double)elapsed.Ticks / totalElapsed.Ticks * 100;
                }
                else
                {
                    m.PercentTotal = 0;
                }
            }
            else
            {
                m.TotalElapsed = new TimeSpan(0);
                m.MinElapsed = new TimeSpan(0);
                m.MaxElapsed = new TimeSpan(0);
                m.AverageElapsed = new TimeSpan(0);
                m.PercentTotal = 0;
            }

            return m;
        }

    }

    public class Metrics
    {
        public int Count { get; set; }
        public TimeSpan TotalElapsed { get; set; }
        public TimeSpan MinElapsed { get; set; }
        public TimeSpan MaxElapsed { get; set; }
        public TimeSpan AverageElapsed { get; set; }
        public double PercentTotal { get; set; }
    }
}
