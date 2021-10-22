using Microsoft.EntityFrameworkCore.Migrations;

namespace vvcard.Migrations
{
    public partial class add_User_Register_LastLoginDatatime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastLogInData",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterData",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLogInData",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegisterData",
                table: "AspNetUsers");
        }
    }
}
