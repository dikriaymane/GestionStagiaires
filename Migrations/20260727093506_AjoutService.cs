using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStagiaires.Migrations
{
    /// <inheritdoc />
    public partial class AjoutService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Service",
                table: "Stagiaires",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Service",
                table: "Stagiaires");
        }
    }
}
