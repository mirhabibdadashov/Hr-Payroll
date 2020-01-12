using Microsoft.EntityFrameworkCore.Migrations;

namespace HRPayrollSystem.Migrations
{
    public partial class OldWorkHoldingNameAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HoldingName",
                table: "OldWorks",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HoldingName",
                table: "OldWorks",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
