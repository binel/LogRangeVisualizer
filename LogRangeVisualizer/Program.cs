using System.Text.Json;

namespace LogRangeVisualizer
{
    internal class Program
    {
        const int PADDING_PIXELS = 5;
        const int TIMELINE_HEIGHT_PIXELS = 100;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: LogRangeVisualizer.exe <path-to-config>");
                return;
            }

            string configText = File.ReadAllText(args[0]);
            Configuration config = JsonSerializer.Deserialize<Configuration>(configText);

            Init(config);
            Write(config, "output.svg");
        }

        private static void Init(Configuration config)
        {
            // Add the utc timeline 
            var utcTimeline = new Timeline
            {
                TimeZoneString = "UTC"
            };
            config.Timelines.Insert(0, utcTimeline);

            // Set the start and end time of each timeline, and the padding/offset
            int verticalOffset = 0;
            foreach (var t in config.Timelines)
            {
                t.StartDateTimeUtc = config.StartDateTimeUtc;
                t.EndDateTimeUtc = config.EndDateTimeUtc;
                t.HorizontalOffsetPixels = PADDING_PIXELS;
                t.VerticalOffsetPixels = verticalOffset;
                verticalOffset += TIMELINE_HEIGHT_PIXELS;
            }

            // Set the parent timeline and configure log days 
            foreach (var l in config.LogDays)
            {
                l.ParentTimeline = utcTimeline;
                l.HorizontalOffsetPixels = PADDING_PIXELS;
                l.VerticalOffsetPixels = verticalOffset;
            } 
        }

        private static void Write(Configuration config, string filename)
        {
            SvgWriter writer = new SvgWriter(filename);

            int width = config.Timelines.Max(t => t.TimelineWidthPixels) + 75;
            int height = (config.Timelines.Count + 1) * TIMELINE_HEIGHT_PIXELS;

            writer.WriteHeader(width, height);

            writer.WriteBackground(width, height);

            foreach (var timeline in config.Timelines)
            {
                timeline.Write(writer);
            }
            foreach (var logDay in config.LogDays)
            {
                logDay.Write(writer);
            }

            writer.Cleanup();
        }
    }
}