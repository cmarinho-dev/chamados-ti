using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChamadosTI.Migrations
{
    /// <inheritdoc />
    public partial class AprimorarFluxoChamados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tecnicosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false)
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tecnicosti", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "DescricaoProblema",
                table: "chamados",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "InventarioItemId",
                table: "chamados",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParecerFinal",
                table: "chamados",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "chamados",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Indefinido")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "TecnicoTiId",
                table: "chamados",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chamados_InventarioItemId",
                table: "chamados",
                column: "InventarioItemId");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_TecnicoTiId",
                table: "chamados",
                column: "TecnicoTiId");

            migrationBuilder.CreateIndex(
                name: "IX_tecnicosti_Nome",
                table: "tecnicosti",
                column: "Nome",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_inventarioitems_InventarioItemId",
                table: "chamados",
                column: "InventarioItemId",
                principalTable: "inventarioitems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_chamados_tecnicosti_TecnicoTiId",
                table: "chamados",
                column: "TecnicoTiId",
                principalTable: "tecnicosti",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chamados_inventarioitems_InventarioItemId",
                table: "chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_chamados_tecnicosti_TecnicoTiId",
                table: "chamados");

            migrationBuilder.DropTable(name: "tecnicosti");

            migrationBuilder.DropIndex(
                name: "IX_chamados_InventarioItemId",
                table: "chamados");

            migrationBuilder.DropIndex(
                name: "IX_chamados_TecnicoTiId",
                table: "chamados");

            migrationBuilder.DropColumn(name: "DescricaoProblema", table: "chamados");
            migrationBuilder.DropColumn(name: "InventarioItemId", table: "chamados");
            migrationBuilder.DropColumn(name: "ParecerFinal", table: "chamados");
            migrationBuilder.DropColumn(name: "Periodo", table: "chamados");
            migrationBuilder.DropColumn(name: "TecnicoTiId", table: "chamados");
        }
    }
}
