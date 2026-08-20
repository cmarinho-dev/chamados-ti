using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChamadosTI.Migrations
{
    /// <inheritdoc />
    public partial class RemoverTabelasRedundantesComponentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO InventarioItemMemoriasQuantidades (InventarioItemId, MemoriaId, Quantidade)
SELECT m.InventarioItemsId, m.MemoriasId, 1
FROM InventarioItemMemorias m
WHERE NOT EXISTS (
    SELECT 1
    FROM InventarioItemMemoriasQuantidades q
    WHERE q.InventarioItemId = m.InventarioItemsId
      AND q.MemoriaId = m.MemoriasId
);");

            migrationBuilder.Sql(@"
INSERT INTO InventarioItemArmazenamentosQuantidades (InventarioItemId, ArmazenamentoId, Quantidade)
SELECT a.InventarioItemsId, a.ArmazenamentosId, 1
FROM InventarioItemArmazenamentos a
WHERE NOT EXISTS (
    SELECT 1
    FROM InventarioItemArmazenamentosQuantidades q
    WHERE q.InventarioItemId = a.InventarioItemsId
      AND q.ArmazenamentoId = a.ArmazenamentosId
);");

            migrationBuilder.Sql(@"
INSERT INTO InventarioItemPerifericosQuantidades (InventarioItemId, PerifericoId, Quantidade)
SELECT p.InventarioItemsId, p.PerifericosId, 1
FROM InventarioItemPerifericos p
WHERE NOT EXISTS (
    SELECT 1
    FROM InventarioItemPerifericosQuantidades q
    WHERE q.InventarioItemId = p.InventarioItemsId
      AND q.PerifericoId = p.PerifericosId
);");

            migrationBuilder.Sql(@"
INSERT INTO InventarioItemCabosQuantidades (InventarioItemId, CaboId, Quantidade)
SELECT c.InventarioItemsId, c.CabosId, 1
FROM InventarioItemCabos c
WHERE NOT EXISTS (
    SELECT 1
    FROM InventarioItemCabosQuantidades q
    WHERE q.InventarioItemId = c.InventarioItemsId
      AND q.CaboId = c.CabosId
);");

            migrationBuilder.DropTable(
                name: "InventarioItemArmazenamentos");

            migrationBuilder.DropTable(
                name: "InventarioItemCabos");

            migrationBuilder.DropTable(
                name: "InventarioItemMemorias");

            migrationBuilder.DropTable(
                name: "InventarioItemPerifericos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioItemArmazenamentos",
                columns: table => new
                {
                    ArmazenamentosId = table.Column<int>(type: "int", nullable: false),
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemArmazenamentos", x => new { x.ArmazenamentosId, x.InventarioItemsId });
                    table.ForeignKey(
                        name: "FK_InventarioItemArmazenamentos_InventarioArmazenamentos_Armaze~",
                        column: x => x.ArmazenamentosId,
                        principalTable: "InventarioArmazenamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemArmazenamentos_InventarioItems_InventarioItems~",
                        column: x => x.InventarioItemsId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItemCabos",
                columns: table => new
                {
                    CabosId = table.Column<int>(type: "int", nullable: false),
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemCabos", x => new { x.CabosId, x.InventarioItemsId });
                    table.ForeignKey(
                        name: "FK_InventarioItemCabos_InventarioCabos_CabosId",
                        column: x => x.CabosId,
                        principalTable: "InventarioCabos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemCabos_InventarioItems_InventarioItemsId",
                        column: x => x.InventarioItemsId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItemMemorias",
                columns: table => new
                {
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false),
                    MemoriasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemMemorias", x => new { x.InventarioItemsId, x.MemoriasId });
                    table.ForeignKey(
                        name: "FK_InventarioItemMemorias_InventarioItems_InventarioItemsId",
                        column: x => x.InventarioItemsId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemMemorias_InventarioMemorias_MemoriasId",
                        column: x => x.MemoriasId,
                        principalTable: "InventarioMemorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItemPerifericos",
                columns: table => new
                {
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false),
                    PerifericosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemPerifericos", x => new { x.InventarioItemsId, x.PerifericosId });
                    table.ForeignKey(
                        name: "FK_InventarioItemPerifericos_InventarioItems_InventarioItemsId",
                        column: x => x.InventarioItemsId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemPerifericos_InventarioPerifericos_PerifericosId",
                        column: x => x.PerifericosId,
                        principalTable: "InventarioPerifericos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemArmazenamentos_InventarioItemsId",
                table: "InventarioItemArmazenamentos",
                column: "InventarioItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemCabos_InventarioItemsId",
                table: "InventarioItemCabos",
                column: "InventarioItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemMemorias_MemoriasId",
                table: "InventarioItemMemorias",
                column: "MemoriasId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemPerifericos_PerifericosId",
                table: "InventarioItemPerifericos",
                column: "PerifericosId");
        }
    }
}
