using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStagiaires.Migrations
{
    /// <inheritdoc />
    public partial class AjoutEntrepriseTuteur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tuteur",
                table: "Stagiaires",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tuteur",
                table: "Stagiaires");
        }
    }
}
