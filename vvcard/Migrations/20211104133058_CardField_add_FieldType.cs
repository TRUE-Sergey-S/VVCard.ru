using Microsoft.EntityFrameworkCore.Migrations;

namespace vvcard.Migrations
{
    public partial class CardField_add_FieldType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldType",
                table: "CardFields",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "CardFields");
        }
    }
}
