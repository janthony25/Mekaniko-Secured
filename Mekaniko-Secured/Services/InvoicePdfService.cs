using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Services
{
    public class InvoicePdfService : IInvoicePdfService
    {
        public Document CreateInvoicePdf(InvoiceDetailsDto invoice)
        {
            var document = new Document();
            var section = document.AddSection();

            // Set page margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);

            AddHeader(section);
            AddInvoiceDetails(section, invoice);
            AddCompanyDetails(section);
            AddInvoiceItemsTable(section, invoice);
            AddInvoiceTotal(section, invoice);
            AddNotes(section, invoice);

            return document;
        }

        private void AddHeader(Section section)
        {
            var header = section.AddParagraph("Mobile Mekaniko Invoice");
            header.Format.Font.Size = 16;
            header.Format.Font.Bold = true;
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.SpaceAfter = 10;
        }

        private void AddInvoiceDetails(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(9));  // Left column for customer details
            table.AddColumn(Unit.FromCentimeter(8));  // Right column for invoice details

            // First row: Customer Name and Invoice Number
            var row = table.AddRow();
            var leftCell = row.Cells[0];
            var rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Customer Name:", invoice.CustomerName);
            AddParagraphWithLabel(rightCell, "Invoice #:", invoice.InvoiceId.ToString(), ParagraphAlignment.Right);

            // Second row: Email Address and Date Issued
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Email Address:", invoice.CustomerEmail);
            AddParagraphWithLabel(rightCell, "Date Issued:", invoice.DateAdded?.ToString("MM/dd/yyyy"), ParagraphAlignment.Right);

            // Third row: Contact Number and Due Date
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Contact #:", invoice.CustomerNumber);
            AddParagraphWithLabel(rightCell, "Due Date:", invoice.DueDate?.ToString("MM/dd/yyyy"), ParagraphAlignment.Right);

            // Fourth row: Address
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Address:", "\"Customer Address\"");
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // **Blank row for spacing before Car Rego**
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];
            leftCell.AddParagraph(" "); // Add a blank space for visual separation
            rightCell.AddParagraph(" "); // Keep right cell empty to maintain alignment

            // Fifth row: Car Rego
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Rego:", invoice.CarRego);
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // Sixth row: Car Model
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Model:", invoice.CarModel);
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // Seventh row: Car Year
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Year:", invoice.CarYear.ToString());
            rightCell.AddParagraph(""); // Empty cell to balance layout

            section.AddParagraph(); // Add space after the table
        }

        private void AddParagraphWithLabel(Cell cell, string label, string value, ParagraphAlignment alignment = ParagraphAlignment.Left)
        {
            var paragraph = cell.AddParagraph();
            paragraph.AddFormattedText(label, TextFormat.Bold);
            paragraph.AddText(" ");
            paragraph.AddText(value);
            paragraph.Format.Alignment = alignment;
        }



        private void AddCompanyDetails(Section section)
        {
            var paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("Mobile Mekaniko\n");
            paragraph.AddText("\"71 Glendale Rd Glen Eden, Auckland\"\n");
            paragraph.AddText("\"027 266 6146\"\n");
            paragraph.AddText("\"00-0000-0000000-000\"");
            section.AddParagraph();
        }

        private void AddInvoiceItemsTable(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Width = 0.5;
            table.Borders.Color = Colors.Black;
            table.AddColumn(Unit.FromCentimeter(8));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));
            table.AddColumn(Unit.FromCentimeter(3));

            var headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray;
            AddTableCell(headerRow.Cells[0], "Item Name", ParagraphAlignment.Left, true);
            AddTableCell(headerRow.Cells[1], "Quantity", ParagraphAlignment.Center, true);
            AddTableCell(headerRow.Cells[2], "Price", ParagraphAlignment.Center, true);
            AddTableCell(headerRow.Cells[3], "Total", ParagraphAlignment.Center, true);

            foreach (var item in invoice.InvoiceItems)
            {
                var row = table.AddRow();
                AddTableCell(row.Cells[0], item.ItemName, ParagraphAlignment.Left);
                AddTableCell(row.Cells[1], item.Quantity.ToString(), ParagraphAlignment.Center);
                AddTableCell(row.Cells[2], item.ItemPrice.ToString("C"), ParagraphAlignment.Right);
                AddTableCell(row.Cells[3], item.ItemTotal?.ToString("C"), ParagraphAlignment.Right);
            }

            section.AddParagraph();
        }

        private void AddInvoiceTotal(Section section, InvoiceDetailsDto invoice)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(13));
            table.AddColumn(Unit.FromCentimeter(4));

            AddTableRow(table, "Subtotal:", invoice.SubTotal?.ToString("C") ?? "N/A");
            AddTableRow(table, "Labor Price:", invoice.LaborPrice?.ToString("C") ?? "N/A");
            AddTableRow(table, "Discount:", invoice.Discount?.ToString("C") ?? "N/A");
            AddTableRow(table, "Shipping Fee:", invoice.ShippingFee?.ToString("C") ?? "N/A");
            AddTableRow(table, "Total Amount:", invoice.TotalAmount?.ToString("C") ?? "N/A");
            AddTableRow(table, "Amount Paid:", invoice.AmountPaid?.ToString("C") ?? "N/A");
            AddTableRow(table, "Payment Status:", invoice.PaymentTerm);

            section.AddParagraph();
        }

        private void AddNotes(Section section, InvoiceDetailsDto invoice)
        {
            // Adding significant space before the "Full Service Details"
            section.AddParagraph().Format.SpaceBefore = 30;

            var paragraph = section.AddParagraph("Full Service Details:");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceAfter = 10;

            section.AddParagraph(invoice.Notes);
        }

        private void AddTableRow(Table table, string label, string value)
        {
            var row = table.AddRow();
            row.Cells[0].AddParagraph(label).Format.Font.Bold = true;
            row.Cells[1].AddParagraph(value);

            // Only attempt to merge cells if the row has enough cells
            if (row.Cells.Count > 2)
            {
                row.Cells[2].MergeRight = 1;  // Merging the last two cells for a cleaner look
            }
        }

        private void AddTableCell(Cell cell, string text, ParagraphAlignment alignment, bool isBold = false)
        {
            var paragraph = cell.AddParagraph(text);
            paragraph.Format.Alignment = alignment;
            if (isBold) paragraph.Format.Font.Bold = true;
        }
    }
}
