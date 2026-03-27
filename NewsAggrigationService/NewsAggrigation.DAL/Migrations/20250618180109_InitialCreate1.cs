using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggrigation.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ExternalAPIConfigs",
                newName: "ApiUrl");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnable",
                table: "ExternalAPIConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "ExternalAPIConfigs");

            migrationBuilder.RenameColumn(
                name: "ApiUrl",
                table: "ExternalAPIConfigs",
                newName: "Status");
        }
    }
}
