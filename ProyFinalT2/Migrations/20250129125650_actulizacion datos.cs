using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyFinalT2.Migrations
{
    /// <inheritdoc />
    public partial class actulizaciondatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Tableros",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "IdUsuarioPropietario",
                table: "Tableros",
                newName: "Orden");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacionId",
                table: "Tableros",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tableros_UsuarioCreacionId",
                table: "Tableros",
                column: "UsuarioCreacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tableros_AspNetUsers_UsuarioCreacionId",
                table: "Tableros",
                column: "UsuarioCreacionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tableros_AspNetUsers_UsuarioCreacionId",
                table: "Tableros");

            migrationBuilder.DropIndex(
                name: "IX_Tableros_UsuarioCreacionId",
                table: "Tableros");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Tableros");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Tableros",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Orden",
                table: "Tableros",
                newName: "IdUsuarioPropietario");
        }
    }
}
