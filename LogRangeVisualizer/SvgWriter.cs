using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogRangeVisualizer
{
    public class SvgWriter
    {
        private readonly XmlWriter _writer;

        public SvgWriter(string outputFilename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            _writer = XmlWriter.Create(outputFilename, settings);
        }

        public SvgWriter(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            _writer = XmlWriter.Create(stream, settings);
        }

        public void WriteHeader(int width, int height)
        {
            _writer.WriteStartElement("svg", "http://www.w3.org/2000/svg");
            _writer.WriteAttributeString("width", width.ToString());
            _writer.WriteAttributeString("height", height.ToString());
            _writer.WriteAttributeString("viewBox", $"0 0 {width} {height}");
        }

        public void WriteGraphics(string fillColor)
        {
            _writer.WriteStartElement("g");
            _writer.WriteAttributeString("fill", fillColor);
        }

        public void EndElement()
        {
            _writer.WriteEndElement();
        }

        public void WriteRect(int x, int y, int width, int height)
        {
            _writer.WriteStartElement("rect");
            _writer.WriteAttributeString("x", x.ToString());
            _writer.WriteAttributeString("y", y.ToString());
            _writer.WriteAttributeString("width", width.ToString());
            _writer.WriteAttributeString("height", height.ToString());
            _writer.WriteEndElement();
        }

        public void WriteLine(int start_x, int start_y, int end_x, int end_y, string color)
        {
            _writer.WriteStartElement("line");
            _writer.WriteAttributeString("x1", start_x.ToString());
            _writer.WriteAttributeString("y1", start_y.ToString());
            _writer.WriteAttributeString("x2", end_x.ToString());
            _writer.WriteAttributeString("y2", end_y.ToString());
            _writer.WriteAttributeString("stroke", color);
            _writer.WriteEndElement();
        }

        public void WriteOpaqueLine(int start_x, int start_y, int end_x, int end_y, string color, double opacity)
        {
            _writer.WriteStartElement("line");
            _writer.WriteAttributeString("x1", start_x.ToString());
            _writer.WriteAttributeString("y1", start_y.ToString());
            _writer.WriteAttributeString("x2", end_x.ToString());
            _writer.WriteAttributeString("y2", end_y.ToString());
            _writer.WriteAttributeString("stroke", color);
            _writer.WriteAttributeString("opacity", opacity.ToString());
            _writer.WriteEndElement();
        }

        public void WritePath(string path, string? fillColor = null)
        {
            _writer.WriteStartElement("path");
            _writer.WriteAttributeString("d", path);

            if (fillColor != null)
            {
                _writer.WriteAttributeString("fill", fillColor);
            }

            _writer.WriteEndElement();
        }

        public void WriteText(int x, int y, string color, string text)
        {
            _writer.WriteStartElement("text");

            _writer.WriteAttributeString("x", x.ToString());
            _writer.WriteAttributeString("y", y.ToString());
            _writer.WriteAttributeString("fill", color);
            _writer.WriteString(text);
            _writer.WriteEndElement();
        }

        public void WriteTextProportional(int x_percent, int y_percent, string color, string text)
        {
            _writer.WriteStartElement("text");

            _writer.WriteAttributeString("x", $"{x_percent}%");
            _writer.WriteAttributeString("y", $"{y_percent}%");
            _writer.WriteAttributeString("fill", color);
            _writer.WriteString(text);
            _writer.WriteEndElement();
        }

        public void WriteTextHorizontallyCenteredAt(int x, int y, string color, string text)
        {
            _writer.WriteStartElement("text");

            _writer.WriteAttributeString("x", x.ToString());
            _writer.WriteAttributeString("y", y.ToString());
            _writer.WriteAttributeString("text-anchor", "middle");
            _writer.WriteAttributeString("fill", color);
            _writer.WriteString(text);
            _writer.WriteEndElement();
        }

        public void Cleanup()
        {
            if (_writer != null)
            {
                _writer.Close();
            }
        }
    }
}
