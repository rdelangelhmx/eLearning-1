using Microsoft.EntityFrameworkCore.Migrations;

namespace eLearning.Migrations
{
    public partial class m3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Course_id",
                table: "LicenseKey",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course_id",
                table: "LicenseKey");
        }
    }
}
