using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fulbito.Core.Migrations
{
    /// <inheritdoc />
    public partial class Streak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinLossStreakToActivate",
                table: "Leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinWinStreakToActivate",
                table: "Leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinLossStreakToActivate",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "MinWinStreakToActivate",
                table: "Leagues");
        }
    }
}
