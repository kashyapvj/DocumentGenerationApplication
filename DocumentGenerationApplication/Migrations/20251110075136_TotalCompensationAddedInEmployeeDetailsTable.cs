using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentGenerationApplication.Migrations
{
    /// <inheritdoc />
    public partial class TotalCompensationAddedInEmployeeDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCompensation",
                table: "EmployeeDetails",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCompensation",
                table: "EmployeeDetails");
        }
    }
}
