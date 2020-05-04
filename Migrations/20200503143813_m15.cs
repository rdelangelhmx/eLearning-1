using Microsoft.EntityFrameworkCore.Migrations;

namespace eLearning.Migrations
{
    public partial class m15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "LicenseKey",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "LicenseKey");
        }
    }
}
