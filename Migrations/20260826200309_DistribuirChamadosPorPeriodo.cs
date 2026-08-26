using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChamadosTI.Migrations
{
    /// <inheritdoc />
    public partial class DistribuirChamadosPorPeriodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdemDistribuicao",
                table: "tecnicosti",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "tecnicosti",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Manhã")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.Sql(@"
                UPDATE tecnicosti
                SET OrdemDistribuicao = FLOOR(RAND() * 999999999) + 1
                WHERE OrdemDistribuicao = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_tecnicosti_Periodo_OrdemDistribuicao",
                table: "tecnicosti",
                columns: new[] { "Periodo", "OrdemDistribuicao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tecnicosti_Periodo_OrdemDistribuicao",
                table: "tecnicosti");

            migrationBuilder.DropColumn(
                name: "OrdemDistribuicao",
                table: "tecnicosti");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "tecnicosti");
        }
    }
}
