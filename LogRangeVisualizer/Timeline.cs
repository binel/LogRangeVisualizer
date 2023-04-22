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
        public const int MINOR_TICK_HEIGHT_PIXELS = 10;
        public const int MAJOR_TICK_HEIGHT_PIXELS = 25;
        public const int TIMELINE_HEIGHT_PIXELS = 100;
        public const int LABEL_HORIZONTAL_MARGIN_PIXELS = 150;
        public const int LABEL_OFFSET_PIXELS = 5;
        public const int MINUTES_PER_PIXEL = 3;

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
        /// How far right on the x axis this timeline should start
        /// </summary>
        [JsonIgnore]
        public int HorizontalOffsetPixels { get; set; }

        /// <summary>
        /// How far down on the y axis this timeline should start 
        /// </summary>
        [JsonIgnore]
        public int VerticalOffsetPixels { get; set; }

        /// <summary>
        /// Width of the timeline in pixels
        /// </summary>
        [JsonIgnore]
        public int TimelineWidthPixels
        {
            get
            {
                var timelineLength = (EndDateTimeUtc - StartDateTimeUtc);
                var numMinutes = (int)Math.Ceiling(timelineLength.TotalMinutes);
                return (numMinutes / MINUTES_PER_PIXEL) + TimelineHorizontalOffset;
            }
        }

        /// <summary>
        /// Where the label should start on the y axis
        /// </summary>
        [JsonIgnore]
        public int LabelVertialOffset
        {
            get
            {
                return (TIMELINE_HEIGHT_PIXELS - LABEL_OFFSET_PIXELS)
                    + VerticalOffsetPixels;
            }
        }

        /// <summary>
        /// Where the label should start on the y axis
        /// </summary>
        [JsonIgnore]
        public int TimelineVerticalOffset
        {
            get
            {
                return (TIMELINE_HEIGHT_PIXELS + VerticalOffsetPixels);
            }
        }

        [JsonIgnore]
        public int TimelineHorizontalOffset
        {
            get
            {
                return (HorizontalOffsetPixels + LABEL_HORIZONTAL_MARGIN_PIXELS);
            }
        }

        public void Write(SvgWriter writer)
        {
            writer.WriteText(HorizontalOffsetPixels, LabelVertialOffset,
                Colors.BLACK, TimeZoneString);
            writer.WriteLine(HorizontalOffsetPixels, TimelineVerticalOffset, TimelineWidthPixels, TimelineVerticalOffset, Colors.BLACK);

            var current = StartDateTimeUtc;
            while (current <= EndDateTimeUtc)
            {
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(current, TimeZone);
                if (localTime.Minute != 0)
                {
                    current = current.AddMinutes(1);
                    continue;
                }
                var horizontalOffset = GetTickHorizontalPosition((int)Math.Ceiling((current - StartDateTimeUtc).TotalMinutes));

                int startingVertial, endingVertical;
                string label;
                if (localTime.Hour != 0)
                {

                    label = localTime.Hour.ToString();
                    startingVertial = TimelineVerticalOffset - MINOR_TICK_HEIGHT_PIXELS;
                    endingVertical = TimelineVerticalOffset + MINOR_TICK_HEIGHT_PIXELS;
                }
                else 
                {
                    label = localTime.ToShortDateString();
                    startingVertial = TimelineVerticalOffset - MAJOR_TICK_HEIGHT_PIXELS;
                    endingVertical = TimelineVerticalOffset + MAJOR_TICK_HEIGHT_PIXELS;
                }

                writer.WriteLine(horizontalOffset, startingVertial,
                    horizontalOffset, endingVertical, Colors.BLACK);
                
                writer.WriteTextHorizontallyCenteredAt(horizontalOffset,
                    startingVertial - 2, Colors.BLACK, label);

                current = current.AddMinutes(1);
            }
        }

        private int GetTickHorizontalPosition(int minutesElapsed)
        {
            return TimelineHorizontalOffset + (minutesElapsed / MINUTES_PER_PIXEL);
        }
    }
}
