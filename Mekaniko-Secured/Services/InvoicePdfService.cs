using Mekaniko_Secured.Models.Dto;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace Mekaniko_Secured.Services
{
    public class InvoicePdfService : IInvoicePdfService
    {
        public Document CreateInvoicePdf(InvoiceDetailsDto invoice)
        {
            var document = new Document();
            var section = document.AddSection();

            AddTitle(section);
            AddInvoiceDetails(section, invoice);
            AddCustomerAndCarDetails(section, invoice);
            AddInvoiceItems(section, invoice);
            AddInvoiceTotals(section, invoice);

            return document;
        }
        private void AddTitle(Section section)
        {
            var title = section.AddParagraph("Mobile Mekaniko Invoice");
            title.Format.Font.Size = 20;
            title.Format.Alignment = ParagraphAlignment.Center;
            title.Format.SpaceAfter = 20;
        }

        private void AddInvoiceDetails(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(4));

            AddTableRow(table, "Invoice Number:", invoice.InvoiceId.ToString());
            AddTableRow(table, "Date Issued:", invoice.DateAdded?.ToShortDateString() ?? "Not Available");
            AddTableRow(table, "Due Date:", invoice.DueDate?.ToShortDateString() ?? "Not Available");


            section.AddParagraph();
        }

        private void AddCustomerAndCarDetails(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(4));

            var row = table.AddRow();
            AddCellPair(row, 0, "Customer Name:", invoice.CustomerName);
            AddCellPair(row, 2, "Car Rego:", invoice.CarRego);

            row = table.AddRow();
            AddCellPair(row, 0, "Customer Email:", invoice.CustomerEmail);
            AddCellPair(row, 2, "Car Model:", invoice.CarModel);

            row = table.AddRow();
            AddCellPair(row, 0, "Customer Number:", invoice.CustomerNumber);
            AddCellPair(row, 2, "Car Year:", invoice.CarYear.ToString());

            section.AddParagraph();
        }

        private void AddInvoiceItems(Section section, InvoiceDetailsDto invoice)
        {
            section.AddParagraph("Invoice Items").Format.Font.Bold = true;

            var table = section.AddTable();
            table.Borders.Visible = true;
            table.AddColumn(Unit.FromCentimeter(8));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));

            var headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray;
            AddHeaderCell(headerRow, 0, "Item Name");
            AddHeaderCell(headerRow, 1, "Quantity");
            AddHeaderCell(headerRow, 2, "Price");
            AddHeaderCell(headerRow, 3, "Total");

            foreach (var item in invoice.InvoiceItems)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(item.ItemName);
                row.Cells[1].AddParagraph(item.Quantity.ToString());
                row.Cells[2].AddParagraph(item.ItemPrice.ToString("C"));
                row.Cells[3].AddParagraph(item.ItemTotal?.ToString("C"));
            }

            section.AddParagraph();
        }

        private void AddInvoiceTotals(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(13));
            table.AddColumn(Unit.FromCentimeter(4));

            AddTableRow(table, "Subtotal:", invoice.SubTotal?.ToString("C"));
            AddTableRow(table, "Labor Price:", invoice.LaborPrice?.ToString("C"));
            AddTableRow(table, "Discount:", invoice.Discount?.ToString("C"));
            AddTableRow(table, "Shipping Fee:", invoice.ShippingFee?.ToString("C"));

            var totalRow = table.AddRow();
            totalRow.Cells[0].AddParagraph("Total Amount:").Format.Font.Bold = true;
            totalRow.Cells[1].AddParagraph(invoice.TotalAmount?.ToString("C")).Format.Font.Bold = true;

            AddTableRow(table, "Amount Paid:", invoice.AmountPaid?.ToString("C"));
            AddTableRow(table, "Payment Status:", invoice.IsPaid.HasValue ? (invoice.IsPaid.Value ? "Paid" : "Not Paid") : "Unknown");

        }

        private void AddTableRow(Table table, string label, string value)
        {
            var row = table.AddRow();
            row.Cells[0].AddParagraph(label);
            row.Cells[1].AddParagraph(value);
        }

        private void AddCellPair(Row row, int startIndex, string label, string value)
        {
            row.Cells[startIndex].AddParagraph(label);
            row.Cells[startIndex + 1].AddParagraph(value);
        }

        private void AddHeaderCell(Row row, int index, string text)
        {
            row.Cells[index].AddParagraph(text).Format.Font.Bold = true;
        }
    }
}
