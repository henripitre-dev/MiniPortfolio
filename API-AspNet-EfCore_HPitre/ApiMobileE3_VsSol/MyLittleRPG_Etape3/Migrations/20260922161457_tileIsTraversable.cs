using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG_Etape3.Migrations
{
    /// <inheritdoc />
    public partial class tileIsTraversable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanPass",
                table: "Tile",
                newName: "IsTraversable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsTraversable",
                table: "Tile",
                newName: "CanPass");
        }
    }
}
