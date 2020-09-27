using Microsoft.EntityFrameworkCore.Migrations;

namespace SobelAlgImage.Migrations
{
    public partial class InitModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    SourceOriginal = table.Column<string>(nullable: true),
                    SourceTransformSlower = table.Column<string>(nullable: true),
                    SourceTransformFaster = table.Column<string>(nullable: true),
                    SourceTransformTaskSlower = table.Column<string>(nullable: true),
                    SourceTransformTaskFaster = table.Column<string>(nullable: true),
                    AmountOfThreads = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageModels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageModels");
        }
    }
}
