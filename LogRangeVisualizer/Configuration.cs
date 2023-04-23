using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogRangeVisualizer
{
    /// <summary>
    /// Configuration for the visualizer - the input file is deserialized to
    /// this class.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The start time of the entire visualization 
        /// </summary>
        public DateTime StartDateTimeUtc { get; set; }

        /// <summary>
        /// The end time of the entire visualization 
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }

        /// <summary>
        /// A collection of timelines to display 
        /// </summary>
        public List<Timeline> Timelines { get; set; }

        /// <summary>
        /// A collection of log days to display
        /// </summary>
        public List<LogDay> LogDays { get; set; }
    }
}
