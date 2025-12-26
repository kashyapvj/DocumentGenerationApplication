using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentGenerationApplication.Migrations
{
    /// <inheritdoc />
    public partial class ReimbursementBenefitsDetailsTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReimbursementBenefitsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeeName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChildEducationAllowance = table.Column<int>(type: "int", nullable: false),
                    ChildHostelAllowance = table.Column<int>(type: "int", nullable: false),
                    LeaveTravelAllowance = table.Column<int>(type: "int", nullable: false),
                    BooksPeriodicalsSelfCertification = table.Column<int>(type: "int", nullable: false),
                    SodexoMealCoupon = table.Column<int>(type: "int", nullable: false),
                    FuelCarReimbursement = table.Column<int>(type: "int", nullable: false),
                    DriverReimbursement = table.Column<int>(type: "int", nullable: false),
                    MobileReimbursement = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReimbursementBenefitsDetails", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReimbursementBenefitsDetails");
        }
    }
}
