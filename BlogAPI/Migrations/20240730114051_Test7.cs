using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAPI.Migrations
{
    public partial class Test7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "LikeDislikes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LikeDislikes_CommentId",
                table: "LikeDislikes",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeDislikes_Comments_CommentId",
                table: "LikeDislikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikeDislikes_Comments_CommentId",
                table: "LikeDislikes");

            migrationBuilder.DropIndex(
                name: "IX_LikeDislikes_CommentId",
                table: "LikeDislikes");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "LikeDislikes");
        }
    }
}
