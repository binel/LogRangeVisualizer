using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogRangeVisualizer
{
    public class LogDay
    {
        public const int TIMELINE_HEIGHT_PIXELS = 100;
        public const int LABEL_HORIZONTAL_MARGIN_PIXELS = 150;
        public const int MINUTES_PER_PIXEL = 3;

        /// <summary>
        /// The time when the log day should begin.
        /// </summary>
        public DateTime StartDateTimeUtc { get; set; }

        /// <summary>
        /// The time when the log day should end.
        /// </summary>
        public DateTime EndDateTimeUtc { get; set; }

        /// <summary>
        /// The label for this log day. Usually a date.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Left-padding 
        /// </summary>
        [JsonIgnore]
        public int HorizontalOffsetPixels { get; set; }

        /// <summary>
        /// Vertical padding 
        /// </summary>
        [JsonIgnore]
        public int VerticalOffsetPixels { get; set; }

        /// <summary>
        /// Where the timeline should start be on the y axis
        /// </summary>
        [JsonIgnore]
        public int TimelineVerticalOffset
        {
            get
            {
                return (TIMELINE_HEIGHT_PIXELS + VerticalOffsetPixels);
            }
        }

        /// <summary>
        /// This is where the timeline should start from the left 
        /// (The horizontal line will go all the way through, this 
        /// just marks where the ticks will begin) 
        /// </summary>
        [JsonIgnore]
        public int TimelineHorizontalOffset
        {
            get
            {
                return (HorizontalOffsetPixels + LABEL_HORIZONTAL_MARGIN_PIXELS);
            }
        }

        /// <summary>
        /// A bit of a hack. The log day needs to be associated to 
        /// the root UTC timeline in order to write correctly. 
        /// </summary>
        [JsonIgnore]
        public Timeline ParentTimeline { get; set; }

        public void Write(SvgWriter writer)
        {
            int logDayStartPixels = 0;
            int logDayEndPixels = 0;
            var current = ParentTimeline.StartDateTimeUtc;
            while (current <= ParentTimeline.EndDateTimeUtc)
            {
                if (current == StartDateTimeUtc)
                {
                    logDayStartPixels = GetTickHorizontalPosition((int)Math.Ceiling((current - ParentTimeline.StartDateTimeUtc).TotalMinutes));
                    writer.WriteOpaqueLine(logDayStartPixels, ParentTimeline.TimelineVerticalOffset, logDayStartPixels, TimelineVerticalOffset, Colors.BLACK, 0.5);
                }

                if (current == EndDateTimeUtc)
                {
                    logDayEndPixels = GetTickHorizontalPosition((int)Math.Ceiling((current - ParentTimeline.StartDateTimeUtc).TotalMinutes));
                    writer.WriteOpaqueLine(logDayEndPixels, ParentTimeline.TimelineVerticalOffset, logDayEndPixels, TimelineVerticalOffset, Colors.BLACK, 0.5);
                }

                current = current.AddMinutes(1);
            }

            writer.WriteLine(logDayStartPixels, TimelineVerticalOffset, logDayEndPixels, TimelineVerticalOffset, Colors.BLACK);
            writer.WriteTextHorizontallyCenteredAt(Midpoint(logDayStartPixels, logDayEndPixels),
    TimelineVerticalOffset - 5, Colors.BLACK, Label);
        }

        private int GetTickHorizontalPosition(int minutesElapsed)
        {
            return TimelineHorizontalOffset + (minutesElapsed / MINUTES_PER_PIXEL);
        }

        private int Midpoint(int startX, int endX)
        {
            return ((endX - startX) / 2) + startX;
        }
    }
}
