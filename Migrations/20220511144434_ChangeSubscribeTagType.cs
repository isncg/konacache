using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kona.Migrations
{
    public partial class ChangeSubscribeTagType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscribeTags_Tags_TagID",
                table: "SubscribeTags");

            migrationBuilder.RenameColumn(
                name: "TagID",
                table: "SubscribeTags",
                newName: "RawTagID");

            migrationBuilder.RenameIndex(
                name: "IX_SubscribeTags_TagID",
                table: "SubscribeTags",
                newName: "IX_SubscribeTags_RawTagID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribeTags_RawTags_RawTagID",
                table: "SubscribeTags",
                column: "RawTagID",
                principalTable: "RawTags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscribeTags_RawTags_RawTagID",
                table: "SubscribeTags");

            migrationBuilder.RenameColumn(
                name: "RawTagID",
                table: "SubscribeTags",
                newName: "TagID");

            migrationBuilder.RenameIndex(
                name: "IX_SubscribeTags_RawTagID",
                table: "SubscribeTags",
                newName: "IX_SubscribeTags_TagID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribeTags_Tags_TagID",
                table: "SubscribeTags",
                column: "TagID",
                principalTable: "Tags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
