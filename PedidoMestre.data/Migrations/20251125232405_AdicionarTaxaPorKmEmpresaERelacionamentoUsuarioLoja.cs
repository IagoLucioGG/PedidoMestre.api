using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PedidoMestre.data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTaxaPorKmEmpresaERelacionamentoUsuarioLoja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HorarioAbertura",
                table: "Lojas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HorarioFechamento",
                table: "Lojas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Lojas",
                type: "numeric(10,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Lojas",
                type: "numeric(11,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RaioEntrega",
                table: "Lojas",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Enderecos",
                type: "numeric(10,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Enderecos",
                type: "numeric(11,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxaPorKm",
                table: "Empresas",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Bairros",
                type: "numeric(10,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Bairros",
                type: "numeric(11,8)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdLoja",
                table: "Usuarios",
                column: "IdLoja");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Lojas_IdLoja",
                table: "Usuarios",
                column: "IdLoja",
                principalTable: "Lojas",
                principalColumn: "IdLoja",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Lojas_IdLoja",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdLoja",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "HorarioAbertura",
                table: "Lojas");

            migrationBuilder.DropColumn(
                name: "HorarioFechamento",
                table: "Lojas");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Lojas");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Lojas");

            migrationBuilder.DropColumn(
                name: "RaioEntrega",
                table: "Lojas");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "TaxaPorKm",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Bairros");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Bairros");
        }
    }
}
