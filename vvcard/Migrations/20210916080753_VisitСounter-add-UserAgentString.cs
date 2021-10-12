using Microsoft.EntityFrameworkCore.Migrations;

namespace vvcard.Migrations
{
    public partial class VisitСounteraddUserAgentString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAgentString",
                table: "VisitСounters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgentString",
                table: "VisitСounters");
        }
    }
}
