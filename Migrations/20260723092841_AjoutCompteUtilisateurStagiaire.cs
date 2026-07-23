using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStagiaires.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCompteUtilisateurStagiaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Stagiaires",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stagiaires_UserId",
                table: "Stagiaires",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Stagiaires_AspNetUsers_UserId",
                table: "Stagiaires",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stagiaires_AspNetUsers_UserId",
                table: "Stagiaires");

            migrationBuilder.DropIndex(
                name: "IX_Stagiaires_UserId",
                table: "Stagiaires");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Stagiaires");
        }
    }
}
