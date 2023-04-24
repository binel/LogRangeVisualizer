using System.Text.Json;

namespace LogRangeVisualizer
{
    public class Program
    {
        const int PADDING_PIXELS = 5;
        const int TIMELINE_HEIGHT_PIXELS = 75;

        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: LogRangeVisualizer.exe <path-to-config> <output-file-name>");
                return;
            }

            string configText = File.ReadAllText(args[0]);
            Configuration config = JsonSerializer.Deserialize<Configuration>(configText);

            Init(config);
            Write(config, args[1]);
        }

        private static void Init(Configuration config)
        {
            // Check that there is a UTC timline. If there isn't, add one
            Timeline utcTimeline = config.Timelines.Where(t => t.TimeZoneString == "UTC").FirstOrDefault();

            if (utcTimeline == null)
            {
                utcTimeline = new Timeline
                {
                    TimeZoneString = "UTC"
                };
                config.Timelines.Insert(0, utcTimeline);
            }

            foreach (var t in config.Timelines)
            {
                t.StartDateTimeUtc = config.StartDateTimeUtc;
                t.EndDateTimeUtc = config.EndDateTimeUtc;
            }
            foreach (var l in config.LogDays)
            {
                l.ParentTimeline = utcTimeline;
            } 
        }

        private static void Write(Configuration config, string filename)
        {
            SvgWriter writer = new SvgWriter(filename);

            // Initialize timeline writers
            List<TimelineWriter> tlWriters = new List<TimelineWriter>();
            int verticalOffset = 0;
            foreach (var timeline in config.Timelines)
            {
                TimelineWriter tlWriter = new TimelineWriter(timeline);

                tlWriter.HorizontalOffsetPixels = PADDING_PIXELS;
                tlWriter.VerticalOffsetPixels = verticalOffset;
                tlWriter.TimelineHeightPixels = TIMELINE_HEIGHT_PIXELS;
                tlWriters.Add(tlWriter);

                verticalOffset += TIMELINE_HEIGHT_PIXELS;
            }

            int width = tlWriters.Max(t => t.TimelineWidthPixels) + 75;
            int height = (config.Timelines.Count + 1) * TIMELINE_HEIGHT_PIXELS + 50;

            writer.WriteHeader(width, height);
            writer.WriteBackground(width, height);

            foreach (var tlwriter in tlWriters)
            {
                tlwriter.Write(writer);
            }
            foreach (var l in config.LogDays)
            {
                l.HorizontalOffsetPixels = PADDING_PIXELS;
                l.VerticalOffsetPixels = verticalOffset;
                l.LogDayHeightPixels = TIMELINE_HEIGHT_PIXELS;
                // hack 
                l.VerticalBoundaryStartingPixel = TIMELINE_HEIGHT_PIXELS;
                l.Write(writer);
            }

            writer.Cleanup();
        }
    }
}