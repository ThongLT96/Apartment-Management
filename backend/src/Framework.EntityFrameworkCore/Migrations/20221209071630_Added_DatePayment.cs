using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Migrations
{
    public partial class Added_DatePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatePayment",
                table: "AbpServiceBill",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePayment",
                table: "AbpApartmentBill",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatePayment",
                table: "AbpServiceBill");

            migrationBuilder.DropColumn(
                name: "DatePayment",
                table: "AbpApartmentBill");
        }
    }
}
