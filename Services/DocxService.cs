using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Xml.Linq;
using System.IO;

namespace FerienspassWebApp.Services
{
    public class DocxService
    {
        public string ConvertToHtml(string filePath)
        {
            byte[] byteArray = File.ReadAllBytes(filePath);

            using var stream = new MemoryStream();
            stream.Write(byteArray, 0, byteArray.Length);
            stream.Position = 0;

            using var doc = WordprocessingDocument.Open(stream, true);

            var settings = new HtmlConverterSettings()
            {
                PageTitle = "Doc"
            };

            XElement html = HtmlConverter.ConvertToHtml(doc, settings);

            return html.ToString();
        }
    }
}