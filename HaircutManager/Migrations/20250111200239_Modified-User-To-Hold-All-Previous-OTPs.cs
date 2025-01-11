using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaircutManager.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedUserToHoldAllPreviousOTPs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otp_AspNetUsers_UserId",
                table: "Otp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Otp",
                table: "Otp");

            migrationBuilder.DropIndex(
                name: "IX_Otp_UserId",
                table: "Otp");

            migrationBuilder.RenameTable(
                name: "Otp",
                newName: "OneTimePasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OneTimePasswords",
                table: "OneTimePasswords",
                columns: new[] { "id", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_OneTimePasswords_UserId",
                table: "OneTimePasswords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OneTimePasswords_AspNetUsers_UserId",
                table: "OneTimePasswords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OneTimePasswords_AspNetUsers_UserId",
                table: "OneTimePasswords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OneTimePasswords",
                table: "OneTimePasswords");

            migrationBuilder.DropIndex(
                name: "IX_OneTimePasswords_UserId",
                table: "OneTimePasswords");

            migrationBuilder.RenameTable(
                name: "OneTimePasswords",
                newName: "Otp");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Otp",
                table: "Otp",
                columns: new[] { "id", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Otp_UserId",
                table: "Otp",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Otp_AspNetUsers_UserId",
                table: "Otp",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
