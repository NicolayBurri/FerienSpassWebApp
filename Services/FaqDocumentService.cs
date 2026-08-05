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
        var paragraphText = new StringBuilder();

        foreach (var child in paragraph.ChildElements)
        {
            switch (child)
            {
                case Run run:
                    paragraphText.Append(ConvertRun(run, mainPart));
                    break;

                case Hyperlink hyperlink:
                    foreach (var run in hyperlink.Elements<Run>())
                    {
                        paragraphText.Append(ConvertRun(run, mainPart));
                    }

                    break;
            }
        }

        if (paragraphText.Length == 0)
        {
            return "<br />";
        }

        var styleId = paragraph
            .ParagraphProperties?
            .ParagraphStyleId?
            .Val?
            .Value;

        var tag = styleId switch
        {
            "Title" => "h2",
            "Heading1" or "berschrift1" => "h2",
            "Heading2" or "berschrift2" => "h3",
            "Heading3" or "berschrift3" => "h4",
            _ => "p"
        };

        return $"<{tag}>{paragraphText}</{tag}>";
    }

    private static string ConvertRun(
        Run run,
        MainDocumentPart mainPart)
    {
        var html = new StringBuilder();

        foreach (var text in run.Elements<Text>())
        {
            var encodedText = WebUtility.HtmlEncode(text.Text);

            if (text.Space?.Value == DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve)
            {
                encodedText = encodedText.Replace(" ", "&nbsp;");
            }

            html.Append(encodedText);
        }

        foreach (var breakElement in run.Elements<Break>())
        {
            html.Append("<br />");
        }

        foreach (var drawing in run.Elements<Drawing>())
        {
            var imageHtml = ConvertDrawingToImage(drawing, mainPart);

            if (!string.IsNullOrWhiteSpace(imageHtml))
            {
                html.Append(imageHtml);
            }
        }

        var content = html.ToString();

        if (string.IsNullOrWhiteSpace(content))
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