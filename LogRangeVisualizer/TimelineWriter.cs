using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogRangeVisualizer
{
    public class TimelineWriter
    {
        public const int MINOR_TICK_HEIGHT_PIXELS = 10;
        public const int MAJOR_TICK_HEIGHT_PIXELS = 25;
        public const int LABEL_HORIZONTAL_MARGIN_PIXELS = 150;
        public const int LABEL_OFFSET_PIXELS = 5;
        public const int MINUTES_PER_PIXEL = 3;

        public Timeline Timeline { get; set; }

        public TimelineWriter(Timeline timeline)
        {
            Timeline = timeline;
        }

        /// <summary>
        /// How far right on the x axis this timeline should start
        /// </summary>
        public int HorizontalOffsetPixels { get; set; }

        /// <summary>
        /// How far down on the y axis this timeline should start 
        /// </summary>
        public int VerticalOffsetPixels { get; set; }

        /// <summary>
        /// Width of the timeline in pixels
        /// </summary>
        public int TimelineWidthPixels
        {
            get
            {
                var timelineLength = (Timeline.EndDateTimeUtc - Timeline.StartDateTimeUtc);
                var numMinutes = (int)Math.Ceiling(timelineLength.TotalMinutes);
                return (numMinutes / MINUTES_PER_PIXEL) + TimelineHorizontalOffset;
            }
        }

        /// <summary>
        /// Where the label should start on the y axis
        /// </summary>
        public int LabelVertialOffset
        {
            get
            {
                return (TimelineHeightPixels - LABEL_OFFSET_PIXELS)
                    + VerticalOffsetPixels;
            }
        }

        /// <summary>
        /// Where the timeline should be on the y axis 
        /// </summary>
        public int TimelineVerticalOffset
        {
            get
            {
                return (TimelineHeightPixels + VerticalOffsetPixels);
            }
        }

        /// <summary>
        /// This is where the timeline should start from the left 
        /// (The horizontal line will go all the way through, this 
        /// just marks where the ticks will begin) 
        /// </summary>
        public int TimelineHorizontalOffset
        {
            get
            {
                return (HorizontalOffsetPixels + LABEL_HORIZONTAL_MARGIN_PIXELS);
            }
        }

        /// <summary>
        /// The vertical height this timeline should take up
        /// </summary>
        public int TimelineHeightPixels { get; set; }

        public void Write(SvgWriter writer)
        {
            writer.WriteText(HorizontalOffsetPixels, LabelVertialOffset,
                Colors.BLACK, Timeline.TimeZoneString);
            writer.WriteLine(HorizontalOffsetPixels, TimelineVerticalOffset, TimelineWidthPixels, TimelineVerticalOffset, Colors.BLACK);

            var current = Timeline.StartDateTimeUtc;
            while (current <= Timeline.EndDateTimeUtc)
            {
                var tick = Timeline.GetTickAtTime(current);
                if (tick == null)
                {
                    current = current.AddMinutes(1);
                    continue;
                }

                var horizontalOffset = GetTickHorizontalPosition((int)Math.Ceiling((current - Timeline.StartDateTimeUtc).TotalMinutes));

                int startingVertial, endingVertical;
                if (tick.IsMinorTick)
                {
                    startingVertial = TimelineVerticalOffset - MINOR_TICK_HEIGHT_PIXELS;
                    endingVertical = TimelineVerticalOffset + MINOR_TICK_HEIGHT_PIXELS;
                }
                else
                {
                    startingVertial = TimelineVerticalOffset - MAJOR_TICK_HEIGHT_PIXELS;
                    endingVertical = TimelineVerticalOffset + MAJOR_TICK_HEIGHT_PIXELS;
                }

                writer.WriteLine(horizontalOffset, startingVertial,
                    horizontalOffset, endingVertical, Colors.BLACK);

                writer.WriteTextHorizontallyCenteredAt(horizontalOffset,
                    startingVertial - 2, Colors.BLACK, tick.Label);

                current = current.AddMinutes(1);
            }
        }

        private int GetTickHorizontalPosition(int minutesElapsed)
        {
            return TimelineHorizontalOffset + (minutesElapsed / MINUTES_PER_PIXEL);
        }
    }
}
