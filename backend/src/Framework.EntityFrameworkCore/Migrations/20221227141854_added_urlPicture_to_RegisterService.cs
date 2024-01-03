using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Migrations
{
    public partial class added_urlPicture_to_RegisterService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlPicture",
                table: "AbpServiceRegister",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlPicture",
                table: "AbpServiceRegister");

        }
    }
}
