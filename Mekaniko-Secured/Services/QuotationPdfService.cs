using Mekaniko_Secured.Models.Dto;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using static System.Collections.Specialized.BitVector32;

namespace Mekaniko_Secured.Services
{
    public class QuotationPdfService : IQuotationPdfService
    {
        public Document CreateQuotationPdf(QuotationDetailsDto quotation)
        {
            var document = new Document();
            var section = document.AddSection();

            // Set page margins
            section.PageSetup.TopMargin = Unit.FromCentimeter(1);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1.5);

            AddHeader(section);
            AddQuotationDetails(section, quotation);
            AddCompanyDetails(section);
            AddQuotationItemsTable(section, quotation);
            AddQuotationTotal(section, quotation);
            AddNotes(section, quotation);

            return document;
        }

        private void AddHeader(MigraDoc.DocumentObjectModel.Section section)
        {
            var header = section.AddParagraph("Mobile Mekaniko Quotation");
            header.Format.Font.Size = 16;
            header.Format.Font.Bold = true;
            header.Format.Alignment = ParagraphAlignment.Center;
            header.Format.SpaceAfter = 10;
        }

        private void AddQuotationDetails(MigraDoc.DocumentObjectModel.Section section, QuotationDetailsDto quotation)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(9));  // Left column for customer details
            table.AddColumn(Unit.FromCentimeter(8));  // Right column for quotation details

            // First row: Customer Name and Quotation Number
            var row = table.AddRow();
            var leftCell = row.Cells[0];
            var rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Customer Name:", quotation.CustomerName);
            AddParagraphWithLabel(rightCell, "Quotation ID:", quotation.QuotationId.ToString(), ParagraphAlignment.Right);

            // Second row: Customer Email and Date
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Customer Email:", quotation.CustomerEmail);
            AddParagraphWithLabel(rightCell, "Date:", quotation.DateAdded?.ToString("MM/dd/yyyy"), ParagraphAlignment.Right);

            // Third row: Customer Number (without right-side content)
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Customer Number:", quotation.CustomerNumber);
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // Add a blank row for spacing after Customer Number
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];
            leftCell.AddParagraph(" ");
            rightCell.AddParagraph(" ");

            // Fourth row: Car Rego
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Rego:", quotation.CarRego);
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // Fifth row: Car Make
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Make:", quotation.MakeName);
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // Sixth row: Car Model
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Model:", quotation.CarModel);
            rightCell.AddParagraph(""); // Empty cell to balance layout

            // Seventh row: Car Year
            row = table.AddRow();
            leftCell = row.Cells[0];
            rightCell = row.Cells[1];

            AddParagraphWithLabel(leftCell, "Car Year:", quotation.CarYear.ToString());
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

        private void AddCompanyDetails(MigraDoc.DocumentObjectModel.Section section)
        {
            var paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("Mobile Mekaniko\n");
            paragraph.AddText("\"71 Glendale Rd Glen Eden, Auckland\"\n");
            paragraph.AddText("\"027 266 6146\"\n");
            paragraph.AddText("\"00-0000-0000000-000\"");
            section.AddParagraph();
        }

        private void AddQuotationItemsTable(MigraDoc.DocumentObjectModel.Section section, QuotationDetailsDto quotation)
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

            foreach (var item in quotation.QuotationItems)
            {
                var row = table.AddRow();
                AddTableCell(row.Cells[0], item.ItemName, ParagraphAlignment.Left);
                AddTableCell(row.Cells[1], item.Quantity.ToString(), ParagraphAlignment.Center);
                AddTableCell(row.Cells[2], item.ItemPrice.ToString("C"), ParagraphAlignment.Right);
                AddTableCell(row.Cells[3], item.ItemTotal?.ToString("C"), ParagraphAlignment.Right);
            }

            section.AddParagraph();
        }

        private void AddQuotationTotal(MigraDoc.DocumentObjectModel.Section section, QuotationDetailsDto quotation)
        {
            var table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn(Unit.FromCentimeter(13));
            table.AddColumn(Unit.FromCentimeter(4));

            AddTableRow(table, "Subtotal:", quotation.SubTotal?.ToString("C") ?? "N/A");
            AddTableRow(table, "Labor Price:", quotation.LaborPrice?.ToString("C") ?? "N/A");
            AddTableRow(table, "Discount:", quotation.Discount?.ToString("C") ?? "N/A");
            AddTableRow(table, "Shipping Fee:", quotation.ShippingFee?.ToString("C") ?? "N/A");
            AddTableRow(table, "Total Amount:", quotation.TotalAmount?.ToString("C") ?? "N/A");

            section.AddParagraph();
        }

        private void AddNotes(MigraDoc.DocumentObjectModel.Section section, QuotationDetailsDto quotation)
        {
            // Adding significant space before the "Note"
            section.AddParagraph().Format.SpaceBefore = 30;

            // Ensure that notes exist before adding them
            if (!string.IsNullOrEmpty(quotation.Notes))
            {
                var paragraph = section.AddParagraph("Note:");
                paragraph.Format.Font.Bold = true;
                paragraph.Format.SpaceAfter = 10;

                // Add the actual note content
                section.AddParagraph(quotation.Notes);
            }
            else
            {
                // Optionally handle the case where there are no notes
                section.AddParagraph("No additional notes provided.");
            }

            // Add a final paragraph stating the validity of the quotation
            var validityParagraph = section.AddParagraph("This quotation is valid for 30 days from the date of issue.");
            validityParagraph.Format.Font.Bold = true;
            validityParagraph.Format.SpaceBefore = 20; // Add space before the final message
        }

        private void AddTableRow(Table table, string label, string value)
        {
            var row = table.AddRow();
            row.Cells[0].AddParagraph(label).Format.Font.Bold = true;
            row.Cells[1].AddParagraph(value);
        }

        private void AddTableCell(Cell cell, string text, ParagraphAlignment alignment, bool isBold = false)
        {
            var paragraph = cell.AddParagraph(text);
            paragraph.Format.Alignment = alignment;
            if (isBold) paragraph.Format.Font.Bold = true;
        }
    }
}
