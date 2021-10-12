using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vvcard.Migrations
{
    public partial class addClickCardField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardFields_Cards_CardId",
                table: "CardFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardFields",
                table: "CardFields");

            migrationBuilder.RenameTable(
                name: "CardFields",
                newName: "CardFields");

            migrationBuilder.RenameColumn(
                name: "FieldValue",
                table: "CardFields",
                newName: "FieldValue");

            migrationBuilder.RenameIndex(
                name: "IX_CardFields_CardId",
                table: "CardFields",
                newName: "IX_CardFields_CardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardFields",
                table: "CardFields",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ClickCardFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardFieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickCardFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClickCardFields_CardFields_CardFieldId",
                        column: x => x.CardFieldId,
                        principalTable: "CardFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClickCardFields_CardFieldId",
                table: "ClickCardFields",
                column: "CardFieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardFields_Cards_CardId",
                table: "CardFields",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardFields_Cards_CardId",
                table: "CardFields");

            migrationBuilder.DropTable(
                name: "ClickCardFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardFields",
                table: "CardFields");

            migrationBuilder.RenameTable(
                name: "CardFields",
                newName: "CardFields");

            migrationBuilder.RenameColumn(
                name: "FieldValue",
                table: "CardFields",
                newName: "FieldValue");

            migrationBuilder.RenameIndex(
                name: "IX_CardFields_CardId",
                table: "CardFields",
                newName: "IX_CardFields_CardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardFields",
                table: "CardFields",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardFields_Cards_CardId",
                table: "CardFields",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
