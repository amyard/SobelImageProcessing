using Microsoft.EntityFrameworkCore.Migrations;

namespace SobelAlgImage.Infrastructure.Data.Migrations
{
    public partial class InitDb : Migration
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
                    SourceGrey50 = table.Column<string>(nullable: true),
                    SourceGrey80 = table.Column<string>(nullable: true),
                    SourceGrey100 = table.Column<string>(nullable: true),
                    SourcConvolutionTasks = table.Column<string>(nullable: true),
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
