using PassportPDF.Api;
using PassportPDF.Client;
using PassportPDF.Model;

namespace BarcodeExtraction
{

    public class BarcodeExtractor
    {
        static async Task Main(string[] args)
        {
            GlobalConfiguration.ApiKey = "YOUR-PASSPORT-CODE";

            PassportManagerApi apiManager = new();
            PassportPDFPassport passportData = await apiManager.PassportManagerGetPassportInfoAsync(GlobalConfiguration.ApiKey);

            if (passportData == null)
            {
                throw new ApiException("The Passport number given is invalid, please set a valid passport number and try again.");
            }
            else if (passportData.IsActive is false)
            {
                throw new ApiException("The Passport number given not active, please go to your PassportPDF dashboard and active your plan.");
            }

            string uri = "https://passportpdfapi.com/test/invoice_with_barcode.pdf";
            
            DocumentApi api = new();

            Console.WriteLine("Loading document into PassportPDF...");
            DocumentLoadResponse document = await api.DocumentLoadFromURIAsync(new LoadDocumentFromURIParameters(uri));
            Console.WriteLine("Document loaded.");

            PDFApi pdfApi = new();

            Console.WriteLine("Launching text recognition process...");
            PdfOCRResponse ocrPdfResponse = await pdfApi.OCRAsync(new PdfOCRParameters(document.FileId, "*")
            {
                Language = "eng",
                SkipPageWithText = false
            });

            if (ocrPdfResponse.Error is not null)
            {
                throw new ApiException(ocrPdfResponse.Error.ExtResultMessage);
            }
            else
            {
                Console.WriteLine("Text recognition process ended.");
            }

            Console.WriteLine("Start text extraction process...");
            PdfExtractTextResponse extractTextResponse = await pdfApi.ExtractTextAsync(new PdfExtractTextParameters(document.FileId, "*")
            {
                TextExtractionMode = PdfExtractTextMode.WholePagePreserveLayout
            });

            Console.WriteLine("Text extracted :");
            foreach (PageText page in extractTextResponse.ExtractedText)
            {
                Console.WriteLine($"======== Page {page.PageNumber} ========");
                Console.WriteLine(page.ExtractedText);
                Console.WriteLine("========================");
            }
        }
    }
}


