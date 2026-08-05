using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FerienspassWebApp.Models;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace FerienspassWebApp.Services;

public class FaqDocumentService
{
    private readonly IWebHostEnvironment _environment;

    public FaqDocumentService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<List<FaqDocument>> LoadFaqDocumentsAsync()
    {
        var folderPath = Path.Combine(
            _environment.ContentRootPath,
            "PrivateFiles",
            "F&Q");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return [];
        }

        var filePaths = Directory
            .EnumerateFiles(folderPath, "*.docx", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName)
            .ToList();

        var documents = new List<FaqDocument>();

        foreach (var filePath in filePaths)
        {
            var document = await Task.Run(() => ReadDocument(filePath));

            if (document is not null)
            {
                documents.Add(document);
            }
        }

        return documents;
    }

    private static FaqDocument? ReadDocument(string filePath)
    {
        try
        {
            using var wordDocument = WordprocessingDocument.Open(
                filePath,
                false);

            var mainPart = wordDocument.MainDocumentPart;

            if (mainPart?.Document?.Body is null)
            {
                return null;
            }

            var question = GetQuestion(wordDocument, filePath);
            var htmlContent = ConvertBodyToHtml(mainPart);

            return new FaqDocument
            {
                Id = Path.GetFileNameWithoutExtension(filePath),
                Question = question,
                HtmlContent = htmlContent,
                FileName = Path.GetFileName(filePath)
            };
        }
        catch
        {
            return null;
        }
    }

    private static string GetQuestion(
        WordprocessingDocument document,
        string filePath)
    {
        var title = document.PackageProperties.Title;

        if (!string.IsNullOrWhiteSpace(title))
        {
            return title.Trim();
        }

        return Path.GetFileNameWithoutExtension(filePath);
    }

    private static string ConvertBodyToHtml(MainDocumentPart mainPart)
    {
        var body = mainPart.Document.Body;

        if (body is null)
        {
            return string.Empty;
        }

        var html = new StringBuilder();

        foreach (var element in body.Elements())
        {
            switch (element)
            {
                case Paragraph paragraph:
                    html.Append(ConvertParagraph(paragraph, mainPart));
                    break;

                case Table table:
                    html.Append(ConvertTable(table, mainPart));
                    break;
            }
        }

        return html.ToString();
    }

    private static string ConvertParagraph(
    Paragraph paragraph,
    MainDocumentPart mainPart)
    {
        var content = new StringBuilder();

        foreach (var child in paragraph.ChildElements)
        {
            switch (child)
            {
                case Run run:
                    content.Append(ConvertRun(run, mainPart));
                    break;

                case Hyperlink hyperlink:
                    foreach (var hyperlinkChild in hyperlink.ChildElements)
                    {
                        if (hyperlinkChild is Run hyperlinkRun)
                        {
                            content.Append(
                                ConvertRun(hyperlinkRun, mainPart));
                        }
                    }

                    break;
            }
        }

        var paragraphContent = content.ToString();

        if (string.IsNullOrWhiteSpace(paragraphContent))
        {
            return "<p class=\"faq-empty-paragraph\"><br /></p>";
        }

        var styleId = paragraph
            .ParagraphProperties?
            .ParagraphStyleId?
            .Val?
            .Value;

        var tag = styleId switch
        {
            "Title" => "h2",

            "Heading1"
                or "berschrift1"
                or "Überschrift1" => "h2",

            "Heading2"
                or "berschrift2"
                or "Überschrift2" => "h3",

            "Heading3"
                or "berschrift3"
                or "Überschrift3" => "h4",

            _ => "p"
        };

        return $"<{tag}>{paragraphContent}</{tag}>";
    }

    private static string ConvertRun(
     Run run,
     MainDocumentPart mainPart)
    {
        var html = new StringBuilder();

        // Elemente genau in der Reihenfolge verarbeiten,
        // in der sie im Word-Dokument vorkommen.
        foreach (var child in run.ChildElements)
        {
            switch (child)
            {
                case Text text:
                    {
                        var value = text.Text
                            // Unsichtbare Word-Trennzeichen entfernen
                            .Replace("\u00AD", string.Empty)
                            .Replace("\u200B", string.Empty);

                        html.Append(WebUtility.HtmlEncode(value));
                        break;
                    }

                case Break:
                    html.Append("<br />");
                    break;

                case CarriageReturn:
                    html.Append("<br />");
                    break;

                case TabChar:
                    html.Append("&emsp;");
                    break;

                case SoftHyphen:
                    // Weiches Trennzeichen nicht ausgeben,
                    // damit Wörter nicht unerwartet getrennt werden.
                    break;

                case Drawing drawing:
                    {
                        var imageHtml = ConvertDrawingToImage(
                            drawing,
                            mainPart);

                        if (!string.IsNullOrWhiteSpace(imageHtml))
                        {
                            html.Append(imageHtml);
                        }

                        break;
                    }
            }
        }

        var content = html.ToString();

        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var properties = run.RunProperties;

        if (properties?.Bold is not null)
        {
            content = $"<strong>{content}</strong>";
        }

        if (properties?.Italic is not null)
        {
            content = $"<em>{content}</em>";
        }

        if (properties?.Underline is not null)
        {
            content = $"<u>{content}</u>";
        }

        return content;
    }

    private static string ConvertDrawingToImage(
        Drawing drawing,
        MainDocumentPart mainPart)
    {
        var blip = drawing
            .Descendants<A.Blip>()
            .FirstOrDefault();

        var relationshipId = blip?.Embed?.Value;

        if (string.IsNullOrWhiteSpace(relationshipId))
        {
            return string.Empty;
        }

        if (mainPart.GetPartById(relationshipId) is not ImagePart imagePart)
        {
            return string.Empty;
        }

        using var stream = imagePart.GetStream();
        using var memoryStream = new MemoryStream();

        stream.CopyTo(memoryStream);

        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        var contentType = imagePart.ContentType;

        var description = drawing
            .Descendants<DW.DocProperties>()
            .FirstOrDefault()?
            .Description?
            .Value;

        var altText = WebUtility.HtmlEncode(description ?? "Bild");

        return $"""
                <img src="data:{contentType};base64,{base64}"
                     alt="{altText}"
                     class="faq-word-image" />
                """;
    }

    private static string ConvertTable(
        Table table,
        MainDocumentPart mainPart)
    {
        var html = new StringBuilder();

        html.Append("<div class=\"faq-table-wrapper\">");
        html.Append("<table class=\"faq-word-table\">");

        foreach (var row in table.Elements<TableRow>())
        {
            html.Append("<tr>");

            foreach (var cell in row.Elements<TableCell>())
            {
                html.Append("<td>");

                foreach (var paragraph in cell.Elements<Paragraph>())
                {
                    html.Append(ConvertParagraph(paragraph, mainPart));
                }

                html.Append("</td>");
            }

            html.Append("</tr>");
        }

        html.Append("</table>");
        html.Append("</div>");

        return html.ToString();
    }
}