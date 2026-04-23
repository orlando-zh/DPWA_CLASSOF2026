using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    public partial class RenombrarIdCategoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // RENOMBRAR EN LA TABLA CATEGORIAS
            migrationBuilder.RenameColumn(
                name: "Id",           // Nombre actual en la BD
                table: "Categorias",  // Tabla correcta
                newName: "idCategoria" // Nuevo nombre que queremos
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // REVERTIR EL CAMBIO SI FUERA NECESARIO
            migrationBuilder.RenameColumn(
                name: "idCategoria",
                table: "Categorias",
                newName: "Id"
            );
        }
    }
}