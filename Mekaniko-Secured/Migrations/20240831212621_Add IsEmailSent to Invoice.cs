using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mekaniko_Secured.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEmailSenttoInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailSent",
                table: "Invoices",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Bce.jiJRbGMd36cOXJ5XruzbVWarlJGdalRr6fF0z0oaY.JHJkQVe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailSent",
                table: "Invoices");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$1gkKOmHFxz6cADL7S.3L4u0YxhSoWIqYELlPrFE6oUroHB04i3ChO");
        }
    }
}
