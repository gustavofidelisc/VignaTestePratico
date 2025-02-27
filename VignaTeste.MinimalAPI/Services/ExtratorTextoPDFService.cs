using UglyToad.PdfPig;
using System.Text;

namespace VignaTeste.MinimalAPI.Services
{
    public class ExtratorTextoPDFService
    {
        public string ExtrairTextoDoPdf(Stream pdfStream)
        {
            StringBuilder texto = new StringBuilder();

            using (PdfDocument documento = PdfDocument.Open(pdfStream))
            {
                foreach (var pagina in documento.GetPages())
                {
                    texto.Append(pagina.Text);
                }
            }
            return texto.ToString();
        }
    }
}
