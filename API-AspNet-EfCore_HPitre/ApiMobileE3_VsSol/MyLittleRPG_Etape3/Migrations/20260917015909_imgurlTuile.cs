using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG_Etape3.Migrations
{
    /// <inheritdoc />
    public partial class imgurlTuile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpriteUrl",
                table: "Tile",
                newName: "ImgUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "Tile",
                newName: "SpriteUrl");
        }
    }
}
