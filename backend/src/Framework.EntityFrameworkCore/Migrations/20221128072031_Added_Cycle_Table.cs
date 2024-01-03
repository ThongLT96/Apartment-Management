using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Migrations
{
    public partial class Added_Cycle_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cycle",
                table: "AbpServiceBill",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cycle",
                table: "AbpServiceBill");
        }
    }
}
