using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStagiaires.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCoordonneesTuteur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BureauTuteur",
                table: "Stagiaires",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailTuteur",
                table: "Stagiaires",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneTuteur",
                table: "Stagiaires",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BureauTuteur",
                table: "Stagiaires");

            migrationBuilder.DropColumn(
                name: "EmailTuteur",
                table: "Stagiaires");

            migrationBuilder.DropColumn(
                name: "TelephoneTuteur",
                table: "Stagiaires");
        }
    }
}
