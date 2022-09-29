using PassportPDF.Api;
using PassportPDF.Client;


namespace BarcodeExtraction
{

    public class BarcodeExtractor
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.ApiKey = "YOUR-PASSPORT-CODE";
            DocumentApi api = new();

            var uri = "https://passportpdfapi.com/test/invoice_with_barcode.pdf";
            var document = api.DocumentLoadFromURIAsync(new PassportPDF.Model.LoadDocumentFromURIParameters(uri)).Result;

            PDFApi pdfApi = new();
            var ocrPdfResponse = pdfApi.OCRAsync(new PassportPDF.Model.PdfOCRParameters(document.FileId, "*")).Result;

            var extractTextResponse = pdfApi.ExtractTextAsync(new PassportPDF.Model.PdfExtractTextParameters(document.FileId, "*")).Result;

            var documentPages = extractTextResponse.ExtractedText;

            foreach(var page in documentPages)
            {
                Console.WriteLine("Page number : {0}", page.PageNumber);
                Console.WriteLine("Extracted text : {0}", page.ExtractedText);
            }

        }
    }
}


