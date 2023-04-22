using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogRangeVisualizer
{
    public class Configuration
    {
        public DateTime StartDateTimeUtc { get; set; }

        public DateTime EndDateTimeUtc { get; set; }

        public List<Timeline> Timelines { get; set; }

        public List<LogDay> LogDays { get; set; }
    }
}
