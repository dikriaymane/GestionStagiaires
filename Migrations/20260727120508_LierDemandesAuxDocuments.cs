using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStagiaires.Migrations
{
    /// <inheritdoc />
    public partial class LierDemandesAuxDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTraitement",
                table: "DemandesDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentStagiaireId",
                table: "DemandesDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DemandesDocuments_DocumentStagiaireId",
                table: "DemandesDocuments",
                column: "DocumentStagiaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_DemandesDocuments_DocumentsStagiaires_DocumentStagiaireId",
                table: "DemandesDocuments",
                column: "DocumentStagiaireId",
                principalTable: "DocumentsStagiaires",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DemandesDocuments_DocumentsStagiaires_DocumentStagiaireId",
                table: "DemandesDocuments");

            migrationBuilder.DropIndex(
                name: "IX_DemandesDocuments_DocumentStagiaireId",
                table: "DemandesDocuments");

            migrationBuilder.DropColumn(
                name: "DateTraitement",
                table: "DemandesDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentStagiaireId",
                table: "DemandesDocuments");
        }
    }
}
