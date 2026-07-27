using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStagiaires.Migrations
{
    /// <inheritdoc />
    public partial class AjoutDemandesDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemandesDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StagiaireId = table.Column<int>(type: "int", nullable: false),
                    TypeDocument = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDemande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesDocuments_Stagiaires_StagiaireId",
                        column: x => x.StagiaireId,
                        principalTable: "Stagiaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemandesDocuments_StagiaireId",
                table: "DemandesDocuments",
                column: "StagiaireId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemandesDocuments");
        }
    }
}
