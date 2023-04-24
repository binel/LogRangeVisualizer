using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogRangeVisualizer
{
    public class Timeline
    {

        /// <summary>
        /// Timeline label. Usually this will be a time zone name or "Log Day" 
        /// </summary>
        public string TimeZoneString { get; set; }

        /// <summary>
        /// The time zone this time line is in
        /// </summary>
        [JsonIgnore]
        public TimeZoneInfo TimeZone 
        {
            get 
            {
                return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneString);
            }
        }

        /// <summary>
        /// The time when the timeline should begin.
        /// </summary>
        [JsonIgnore]
        public DateTime StartDateTimeUtc { get; set; }

        /// <summary>
        /// The time when the timeline should end.
        /// </summary>
        [JsonIgnore]
        public DateTime EndDateTimeUtc { get; set; }

        /// <summary>
        /// Returns a tickmark if a tick should be shown at the provided utc time.
        /// If no tick mark should be shown, null is returned
        /// </summary>
        public Tickmark GetTickAtTime(DateTime utcTime)
        {
            var localTime = TimeZoneInfo.ConvertTimeToUtc(utcTime, TimeZone);
            if (localTime.Minute != 0)
            {
                return null;
            }

            if (localTime.Hour == 0)
            {
                return new Tickmark
                {
                    IsMinorTick = false,
                    Label = localTime.ToShortDateString()
                };
            }
            else 
            {
                return new Tickmark
                {
                    IsMinorTick = true,
                    Label = localTime.Hour.ToString()
                };
            }
        }
    }
}
