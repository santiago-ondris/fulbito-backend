using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestionLigas.Migrations
{
    /// <inheritdoc />
    public partial class InitialCrearLiga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ligas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UsuarioId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ligas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesLiga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LigaId = table.Column<int>(type: "integer", nullable: false),
                    Formato = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ArqueroObligatorio = table.Column<bool>(type: "boolean", nullable: false),
                    CantidadMinimaArqueros = table.Column<int>(type: "integer", nullable: false),
                    PosicionesDisponibles = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DiasJuego = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Horarios = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReglasAdicionales = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesLiga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesLiga_Ligas_LigaId",
                        column: x => x.LigaId,
                        principalTable: "Ligas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesLiga_LigaId",
                table: "ConfiguracionesLiga",
                column: "LigaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesLiga");

            migrationBuilder.DropTable(
                name: "Ligas");
        }
    }
}
