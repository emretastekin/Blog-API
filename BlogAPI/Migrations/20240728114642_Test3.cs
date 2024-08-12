using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogAPI.Migrations
{
    public partial class Test3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentOfCommentId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentOfCommentId",
                table: "Comments",
                column: "CommentOfCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_CommentOfCommentId",
                table: "Comments",
                column: "CommentOfCommentId",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_CommentOfCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentOfCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentOfCommentId",
                table: "Comments");
        }
    }
}
