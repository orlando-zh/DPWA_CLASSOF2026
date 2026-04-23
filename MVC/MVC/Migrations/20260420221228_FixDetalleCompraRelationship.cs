using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    /// <inheritdoc />
    public partial class FixDetalleCompraRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VideoJuegosId",
                table: "detalle_compra",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DetalleCompraId",
                table: "detalle_compra",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_DetalleCompraId",
                table: "detalle_compra",
                column: "DetalleCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_VideoJuegosId",
                table: "detalle_compra",
                column: "VideoJuegosId");

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_compra_VideoJuegos_VideoJuegosId",
                table: "detalle_compra",
                column: "VideoJuegosId",
                principalTable: "VideoJuegos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_compra_detalle_compra_DetalleCompraId",
                table: "detalle_compra",
                column: "DetalleCompraId",
                principalTable: "detalle_compra",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detalle_compra_VideoJuegos_VideoJuegosId",
                table: "detalle_compra");

            migrationBuilder.DropForeignKey(
                name: "FK_detalle_compra_detalle_compra_DetalleCompraId",
                table: "detalle_compra");

            migrationBuilder.DropIndex(
                name: "IX_detalle_compra_DetalleCompraId",
                table: "detalle_compra");

            migrationBuilder.DropIndex(
                name: "IX_detalle_compra_VideoJuegosId",
                table: "detalle_compra");

            migrationBuilder.DropColumn(
                name: "DetalleCompraId",
                table: "detalle_compra");

            migrationBuilder.AlterColumn<string>(
                name: "VideoJuegosId",
                table: "detalle_compra",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
