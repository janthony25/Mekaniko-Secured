using Mekaniko_Secured.Models.Dto;
using MigraDoc.DocumentObjectModel;

namespace Mekaniko_Secured.Services
{
    public interface IQuotationPdfService
    {
        Document CreateQuotationPdf(QuotationDetailsDto quotation);
    }
}