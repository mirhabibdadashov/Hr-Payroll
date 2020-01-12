using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HRPayrollSystem.Migrations
{
    public partial class CalculatesTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calculates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CalculateDate = table.Column<DateTime>(nullable: false),
                    TotalSalary = table.Column<decimal>(nullable: false),
                    TotalAbsentDays = table.Column<int>(nullable: false),
                    TotalPenalties = table.Column<decimal>(nullable: false),
                    TotalBonuses = table.Column<decimal>(nullable: false),
                    TotalVacationDays = table.Column<decimal>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calculates_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calculates_EmployeeId",
                table: "Calculates",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculates");
        }
    }
}
