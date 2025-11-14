using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentGenerationApplication.Migrations
{
    /// <inheritdoc />
    public partial class WorkinDays_BonusAmount_Probation_AddedColumnsInEmployeeDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "SalaryBreakdowns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OfferValidTill",
                table: "SalaryBreakdownInput",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BonusAmount",
                table: "SalaryBreakdownInput",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Probation",
                table: "SalaryBreakdownInput",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "WorkingDays",
                table: "SalaryBreakdownInput",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "EmployeeDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BonusAmount",
                table: "EmployeeDetails",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Probation",
                table: "EmployeeDetails",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "WorkingDays",
                table: "EmployeeDetails",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusAmount",
                table: "SalaryBreakdownInput");

            migrationBuilder.DropColumn(
                name: "Probation",
                table: "SalaryBreakdownInput");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "SalaryBreakdownInput");

            migrationBuilder.DropColumn(
                name: "BonusAmount",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "Probation",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "EmployeeDetails");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "SalaryBreakdowns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OfferValidTill",
                table: "SalaryBreakdownInput",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "EmployeeDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
