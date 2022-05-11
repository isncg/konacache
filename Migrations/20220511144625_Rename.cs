using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kona.Migrations
{
    public partial class Rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscribeTags_RawTags_RawTagID",
                table: "SubscribeTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscribeTags_Subscribes_SubscribeID",
                table: "SubscribeTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscribeTags",
                table: "SubscribeTags");

            migrationBuilder.RenameTable(
                name: "SubscribeTags",
                newName: "SubscribeRawTags");

            migrationBuilder.RenameIndex(
                name: "IX_SubscribeTags_SubscribeID",
                table: "SubscribeRawTags",
                newName: "IX_SubscribeRawTags_SubscribeID");

            migrationBuilder.RenameIndex(
                name: "IX_SubscribeTags_RawTagID",
                table: "SubscribeRawTags",
                newName: "IX_SubscribeRawTags_RawTagID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscribeRawTags",
                table: "SubscribeRawTags",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribeRawTags_RawTags_RawTagID",
                table: "SubscribeRawTags",
                column: "RawTagID",
                principalTable: "RawTags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribeRawTags_Subscribes_SubscribeID",
                table: "SubscribeRawTags",
                column: "SubscribeID",
                principalTable: "Subscribes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscribeRawTags_RawTags_RawTagID",
                table: "SubscribeRawTags");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscribeRawTags_Subscribes_SubscribeID",
                table: "SubscribeRawTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscribeRawTags",
                table: "SubscribeRawTags");

            migrationBuilder.RenameTable(
                name: "SubscribeRawTags",
                newName: "SubscribeTags");

            migrationBuilder.RenameIndex(
                name: "IX_SubscribeRawTags_SubscribeID",
                table: "SubscribeTags",
                newName: "IX_SubscribeTags_SubscribeID");

            migrationBuilder.RenameIndex(
                name: "IX_SubscribeRawTags_RawTagID",
                table: "SubscribeTags",
                newName: "IX_SubscribeTags_RawTagID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscribeTags",
                table: "SubscribeTags",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribeTags_RawTags_RawTagID",
                table: "SubscribeTags",
                column: "RawTagID",
                principalTable: "RawTags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscribeTags_Subscribes_SubscribeID",
                table: "SubscribeTags",
                column: "SubscribeID",
                principalTable: "Subscribes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
