using Microsoft.EntityFrameworkCore.Migrations;

namespace HRPayrollSystem.Migrations
{
    public partial class GradeEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Grades",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Earnings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Earnings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Earnings");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Earnings");
        }
    }
}
