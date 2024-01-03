using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Migrations
{
    public partial class Added_Unit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AbpApartmentServices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AbpApartmentServices");
        }
    }
}
