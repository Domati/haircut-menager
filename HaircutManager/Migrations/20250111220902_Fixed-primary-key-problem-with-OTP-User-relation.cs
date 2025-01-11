using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaircutManager.Migrations
{
    /// <inheritdoc />
    public partial class FixedprimarykeyproblemwithOTPUserrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OneTimePasswords",
                table: "OneTimePasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OneTimePasswords",
                table: "OneTimePasswords",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OneTimePasswords",
                table: "OneTimePasswords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OneTimePasswords",
                table: "OneTimePasswords",
                columns: new[] { "id", "UserId" });
        }
    }
}
