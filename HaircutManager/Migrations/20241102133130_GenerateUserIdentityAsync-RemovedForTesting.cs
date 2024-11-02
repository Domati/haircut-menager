using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaircutManager.Migrations
{
    /// <inheritdoc />
    public partial class GenerateUserIdentityAsyncRemovedForTesting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OldPasswords",
                table: "OldPasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldPasswords",
                table: "OldPasswords",
                columns: new[] { "id", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OldPasswords",
                table: "OldPasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldPasswords",
                table: "OldPasswords",
                column: "id");
        }
    }
}
