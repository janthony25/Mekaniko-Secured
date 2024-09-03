using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mekaniko_Secured.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$hLr8plYsf/NpuUyeXobr1e3d.3pZjtdUsDA./lGLk4bHiZdtL2h.C");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$1gkKOmHFxz6cADL7S.3L4u0YxhSoWIqYELlPrFE6oUroHB04i3ChO");
        }
    }
}
