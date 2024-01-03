using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Migrations
{
    public partial class added_picture_in_ServiceBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "AbpServiceBill",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "AbpServiceBill");           
        }
    }
}
