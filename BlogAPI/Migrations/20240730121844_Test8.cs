using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAPI.Migrations
{
    public partial class Test8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CategoryID",
                table: "Posts",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "SubCategoryID",
                table: "Posts",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(800)", maxLength: 800, nullable: false),
                    CategoryID = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryID",
                table: "Posts",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SubCategoryID",
                table: "Posts",
                column: "SubCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryID",
                table: "SubCategories",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryID",
                table: "Posts",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_SubCategories_SubCategoryID",
                table: "Posts",
                column: "SubCategoryID",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryID",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_SubCategories_SubCategoryID",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CategoryID",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_SubCategoryID",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SubCategoryID",
                table: "Posts");
        }
    }
}
