using Microsoft.EntityFrameworkCore.Migrations;

namespace eLearning.Migrations
{
    public partial class m21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lecture_Color",
                table: "Lecture",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lecture_Color",
                table: "Lecture");
        }
    }
}
