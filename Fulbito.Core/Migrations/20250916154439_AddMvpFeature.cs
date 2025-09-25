using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fulbito.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMvpFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMvp",
                table: "PlayerMatches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMvpEnabled",
                table: "Leagues",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PointsPerMvp",
                table: "Leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMvp",
                table: "PlayerMatches");

            migrationBuilder.DropColumn(
                name: "IsMvpEnabled",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "PointsPerMvp",
                table: "Leagues");
        }
    }
}
