using Microsoft.EntityFrameworkCore.Migrations;

namespace vvcard.Migrations
{
    public partial class addVisitCountersJsonResponseField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonResponse",
                table: "VisitСounters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonResponse",
                table: "VisitСounters");
        }
    }
}
