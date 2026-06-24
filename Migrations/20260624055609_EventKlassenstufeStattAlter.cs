using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FerienspassWebApp.Migrations
{
    /// <inheritdoc />
    public partial class EventKlassenstufeStattAlter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KlasseMax",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KlasseMin",
                table: "Events",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KlasseMax",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "KlasseMin",
                table: "Events");
        }
    }
}
