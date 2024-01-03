using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Framework.Migrations
{
    public partial class added_FullAuditedEntity_in_AbpUserFeedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AbpFeedback",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "AbpFeedback",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "AbpFeedback",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AbpFeedback",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AbpFeedback",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AbpFeedback",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "AbpFeedback",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AbpFeedback",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AbpFeedback");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "AbpFeedback");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AbpFeedback",
                newName: "ID");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "AbpFeedback",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbpFeedback",
                table: "AbpFeedback",
                column: "ID");
        }
    }
}
