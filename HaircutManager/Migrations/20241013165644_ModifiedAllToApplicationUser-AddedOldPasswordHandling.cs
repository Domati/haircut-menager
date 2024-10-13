using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaircutManager.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAllToApplicationUserAddedOldPasswordHandling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OldPassword_AspNetUsers_UserId",
                table: "OldPassword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldPassword",
                table: "OldPassword");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "OldPassword",
                newName: "OldPasswords");

            migrationBuilder.RenameColumn(
                name: "HashedPassword",
                table: "OldPasswords",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "OldPasswords",
                newName: "ChangedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OldPassword_UserId",
                table: "OldPasswords",
                newName: "IX_OldPasswords_UserId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "LastName",
                keyValue: null,
                column: "LastName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "FirstName",
                keyValue: null,
                column: "FirstName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldPasswords",
                table: "OldPasswords",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldPasswords_AspNetUsers_UserId",
                table: "OldPasswords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OldPasswords_AspNetUsers_UserId",
                table: "OldPasswords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldPasswords",
                table: "OldPasswords");

            migrationBuilder.RenameTable(
                name: "OldPasswords",
                newName: "OldPassword");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "OldPassword",
                newName: "HashedPassword");

            migrationBuilder.RenameColumn(
                name: "ChangedAt",
                table: "OldPassword",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OldPasswords_UserId",
                table: "OldPassword",
                newName: "IX_OldPassword_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldPassword",
                table: "OldPassword",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldPassword_AspNetUsers_UserId",
                table: "OldPassword",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
