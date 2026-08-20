using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChamadosTI.Migrations
{
    /// <inheritdoc />
    public partial class InitChamadosDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Chamados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomeSolicitante = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Setor = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Situacao = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    FinalizadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chamados", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioAntivirus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioAntivirus", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioArmazenamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioArmazenamentos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioCabos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioCabos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioChavesLicencas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Produto = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Chave = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacao = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioChavesLicencas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioConexoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioConexoes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioMemorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioMemorias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioMonitores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InventarioNumero = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Patrimonio = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Marca = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modelo = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Polegadas = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacao = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioMonitores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioOffices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioOffices", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioPerifericos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioPerifericos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioProcessadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descricao = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioProcessadores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioSetores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioSetores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioSistemasOperacionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioSistemasOperacionais", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdministrativoUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Usuario = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativoUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdministrativoUsuarios_InventarioSetores_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "InventarioSetores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InventarioNumero = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Patrimonio = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoEquipamento = table.Column<int>(type: "int", nullable: false),
                    EhBackup = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PessoaResponsavel = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SetorId = table.Column<int>(type: "int", nullable: false),
                    SistemaOperacionalId = table.Column<int>(type: "int", nullable: true),
                    OfficeId = table.Column<int>(type: "int", nullable: true),
                    AntivirusId = table.Column<int>(type: "int", nullable: true),
                    ConexaoId = table.Column<int>(type: "int", nullable: true),
                    Ip = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observacao = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CriadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioItems_InventarioAntivirus_AntivirusId",
                        column: x => x.AntivirusId,
                        principalTable: "InventarioAntivirus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioItems_InventarioConexoes_ConexaoId",
                        column: x => x.ConexaoId,
                        principalTable: "InventarioConexoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioItems_InventarioOffices_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "InventarioOffices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioItems_InventarioSetores_SetorId",
                        column: x => x.SetorId,
                        principalTable: "InventarioSetores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioItems_InventarioSistemasOperacionais_SistemaOperac~",
                        column: x => x.SistemaOperacionalId,
                        principalTable: "InventarioSistemasOperacionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "InventarioItemArmazenamentosQuantidades",
                columns: table => new
                {
                    InventarioItemId = table.Column<int>(type: "int", nullable: false),
                    ArmazenamentoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemArmazenamentosQuantidades", x => new { x.InventarioItemId, x.ArmazenamentoId });
                    table.ForeignKey(
                        name: "FK_InventarioItemArmazenamentosQuantidades_InventarioArmazename~",
                        column: x => x.ArmazenamentoId,
                        principalTable: "InventarioArmazenamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemArmazenamentosQuantidades_InventarioItems_Inve~",
                        column: x => x.InventarioItemId,
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
                name: "InventarioItemCabosQuantidades",
                columns: table => new
                {
                    InventarioItemId = table.Column<int>(type: "int", nullable: false),
                    CaboId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemCabosQuantidades", x => new { x.InventarioItemId, x.CaboId });
                    table.ForeignKey(
                        name: "FK_InventarioItemCabosQuantidades_InventarioCabos_CaboId",
                        column: x => x.CaboId,
                        principalTable: "InventarioCabos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemCabosQuantidades_InventarioItems_InventarioIte~",
                        column: x => x.InventarioItemId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItemChavesLicencas",
                columns: table => new
                {
                    ChavesLicencasId = table.Column<int>(type: "int", nullable: false),
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemChavesLicencas", x => new { x.ChavesLicencasId, x.InventarioItemsId });
                    table.ForeignKey(
                        name: "FK_InventarioItemChavesLicencas_InventarioChavesLicencas_Chaves~",
                        column: x => x.ChavesLicencasId,
                        principalTable: "InventarioChavesLicencas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemChavesLicencas_InventarioItems_InventarioItems~",
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
                name: "InventarioItemMemoriasQuantidades",
                columns: table => new
                {
                    InventarioItemId = table.Column<int>(type: "int", nullable: false),
                    MemoriaId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemMemoriasQuantidades", x => new { x.InventarioItemId, x.MemoriaId });
                    table.ForeignKey(
                        name: "FK_InventarioItemMemoriasQuantidades_InventarioItems_Inventario~",
                        column: x => x.InventarioItemId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemMemoriasQuantidades_InventarioMemorias_Memoria~",
                        column: x => x.MemoriaId,
                        principalTable: "InventarioMemorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItemMonitores",
                columns: table => new
                {
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false),
                    MonitoresId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemMonitores", x => new { x.InventarioItemsId, x.MonitoresId });
                    table.ForeignKey(
                        name: "FK_InventarioItemMonitores_InventarioItems_InventarioItemsId",
                        column: x => x.InventarioItemsId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemMonitores_InventarioMonitores_MonitoresId",
                        column: x => x.MonitoresId,
                        principalTable: "InventarioMonitores",
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

            migrationBuilder.CreateTable(
                name: "InventarioItemPerifericosQuantidades",
                columns: table => new
                {
                    InventarioItemId = table.Column<int>(type: "int", nullable: false),
                    PerifericoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemPerifericosQuantidades", x => new { x.InventarioItemId, x.PerifericoId });
                    table.ForeignKey(
                        name: "FK_InventarioItemPerifericosQuantidades_InventarioItems_Inventa~",
                        column: x => x.InventarioItemId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemPerifericosQuantidades_InventarioPerifericos_P~",
                        column: x => x.PerifericoId,
                        principalTable: "InventarioPerifericos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventarioItemProcessadores",
                columns: table => new
                {
                    InventarioItemsId = table.Column<int>(type: "int", nullable: false),
                    ProcessadoresId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioItemProcessadores", x => new { x.InventarioItemsId, x.ProcessadoresId });
                    table.ForeignKey(
                        name: "FK_InventarioItemProcessadores_InventarioItems_InventarioItemsId",
                        column: x => x.InventarioItemsId,
                        principalTable: "InventarioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioItemProcessadores_InventarioProcessadores_Processa~",
                        column: x => x.ProcessadoresId,
                        principalTable: "InventarioProcessadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "InventarioAntivirus",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Kaspersky" },
                    { 2, "N/T" }
                });

            migrationBuilder.InsertData(
                table: "InventarioArmazenamentos",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "HDD 150GB" },
                    { 2, "HDD 300GB" },
                    { 3, "HDD 500GB" },
                    { 4, "HDD 1TB" },
                    { 5, "HDD 2TB" },
                    { 6, "SSD SATA 120GB" },
                    { 7, "SSD SATA 256GB" },
                    { 8, "SSD SATA 512GB" },
                    { 9, "SSD SATA 1TB" },
                    { 10, "SSD NVME 120GB" },
                    { 11, "SSD NVME 256GB" },
                    { 12, "SSD NVME 512GB" },
                    { 13, "SSD NVME 1TB" }
                });

            migrationBuilder.InsertData(
                table: "InventarioCabos",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "HDMI" },
                    { 2, "VGA" },
                    { 3, "DVI" },
                    { 4, "Alimentacao EU - 3 pinos" },
                    { 5, "Alimentacao EU - 2 pinos" },
                    { 6, "Alimentacao US - 3 pinos" },
                    { 7, "Alimentacao US - 2 pinos" }
                });

            migrationBuilder.InsertData(
                table: "InventarioConexoes",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Ethernet" },
                    { 2, "Wifi" },
                    { 3, "N/T" }
                });

            migrationBuilder.InsertData(
                table: "InventarioMemorias",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "DDR2 2GB" },
                    { 2, "DDR2 4GB" },
                    { 3, "DDR2 8GB" },
                    { 4, "DDR3 4GB" },
                    { 5, "DDR3 8GB" },
                    { 6, "DDR3 16GB" },
                    { 7, "DDR4 4GB" },
                    { 8, "DDR4 8GB" },
                    { 9, "DDR4 16GB" },
                    { 10, "DDR4 32GB" },
                    { 11, "DDR4 64GB" }
                });

            migrationBuilder.InsertData(
                table: "InventarioOffices",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "2007" },
                    { 2, "2010" },
                    { 3, "2013" },
                    { 4, "2016" },
                    { 5, "2019" },
                    { 6, "365" },
                    { 7, "N/T" },
                    { 8, "MacOS" }
                });

            migrationBuilder.InsertData(
                table: "InventarioPerifericos",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Teclado Positivo" },
                    { 2, "Mouse Positivo" },
                    { 3, "Teclado Multilaser" },
                    { 4, "Mouse Multilaser" },
                    { 5, "Teclado Logitech" },
                    { 6, "Mouse Logitech" },
                    { 7, "Webcam Logitech C270" },
                    { 8, "Webcam Logitech C920" }
                });

            migrationBuilder.InsertData(
                table: "InventarioProcessadores",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Intel Core i3 2th" },
                    { 2, "Intel Core i3 3th" },
                    { 3, "Intel Core i3 4th" },
                    { 4, "Intel Core i3 5th" },
                    { 5, "Intel Core i3 6th" },
                    { 6, "Intel Core i3 7th" },
                    { 7, "Intel Core i3 8th" },
                    { 8, "Intel Core i3 9th" },
                    { 9, "Intel Core i3 10th" },
                    { 10, "Intel Core i3 11th" },
                    { 11, "Intel Core i5 5th" },
                    { 12, "Intel Core i5 6th" },
                    { 13, "Intel Core i5 7th" },
                    { 14, "Intel Core i5 8th" },
                    { 15, "Intel Core i5 9th" },
                    { 16, "Intel Core i5 10th" },
                    { 17, "Intel Core i5 11th" },
                    { 18, "Intel Core i5 12th" },
                    { 19, "Intel Core i7 5th" },
                    { 20, "Intel Core i7 6th" },
                    { 21, "Intel Core i7 7th" },
                    { 22, "Intel Core i7 8th" },
                    { 23, "Intel Core i7 9th" },
                    { 24, "Intel Core i7 10th" },
                    { 25, "Intel Core i7 11th" },
                    { 26, "Intel Core i7 12th" }
                });

            migrationBuilder.InsertData(
                table: "InventarioSetores",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "PRESIDENCIA / GABINETE" },
                    { 2, "ASSESSORIA DE COMUNICACAO" },
                    { 3, "ASSESSORIA TECNICA" },
                    { 4, "APAF" },
                    { 5, "APDI" },
                    { 6, "APPLI" },
                    { 7, "BIBLIOTECA" },
                    { 8, "CENTRO DE EVENTOS IMAP BARIGUI" },
                    { 9, "EAP" },
                    { 10, "ESTAGIO" },
                    { 11, "ESTUDIO" },
                    { 12, "NEAD" },
                    { 13, "NIT" },
                    { 14, "SEGURO" },
                    { 15, "RH" }
                });

            migrationBuilder.InsertData(
                table: "InventarioSistemasOperacionais",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Windows XP" },
                    { 2, "Windows 7" },
                    { 3, "Windows 8" },
                    { 4, "Windows 8.1" },
                    { 5, "Windows 10" },
                    { 6, "Windows 11" },
                    { 7, "MacOS" },
                    { 8, "Arlequim/Ubuntu" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativoUsuarios_DepartamentoId",
                table: "AdministrativoUsuarios",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AdministrativoUsuarios_Usuario",
                table: "AdministrativoUsuarios",
                column: "Usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioAntivirus_Nome",
                table: "InventarioAntivirus",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioArmazenamentos_Descricao",
                table: "InventarioArmazenamentos",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioCabos_Descricao",
                table: "InventarioCabos",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioChavesLicencas_Tipo_Chave",
                table: "InventarioChavesLicencas",
                columns: new[] { "Tipo", "Chave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioConexoes_Nome",
                table: "InventarioConexoes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemArmazenamentos_InventarioItemsId",
                table: "InventarioItemArmazenamentos",
                column: "InventarioItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemArmazenamentosQuantidades_ArmazenamentoId",
                table: "InventarioItemArmazenamentosQuantidades",
                column: "ArmazenamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemCabos_InventarioItemsId",
                table: "InventarioItemCabos",
                column: "InventarioItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemCabosQuantidades_CaboId",
                table: "InventarioItemCabosQuantidades",
                column: "CaboId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemChavesLicencas_InventarioItemsId",
                table: "InventarioItemChavesLicencas",
                column: "InventarioItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemMemorias_MemoriasId",
                table: "InventarioItemMemorias",
                column: "MemoriasId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemMemoriasQuantidades_MemoriaId",
                table: "InventarioItemMemoriasQuantidades",
                column: "MemoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemMonitores_MonitoresId",
                table: "InventarioItemMonitores",
                column: "MonitoresId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemPerifericos_PerifericosId",
                table: "InventarioItemPerifericos",
                column: "PerifericosId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemPerifericosQuantidades_PerifericoId",
                table: "InventarioItemPerifericosQuantidades",
                column: "PerifericoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItemProcessadores_ProcessadoresId",
                table: "InventarioItemProcessadores",
                column: "ProcessadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItems_AntivirusId",
                table: "InventarioItems",
                column: "AntivirusId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItems_ConexaoId",
                table: "InventarioItems",
                column: "ConexaoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItems_InventarioNumero",
                table: "InventarioItems",
                column: "InventarioNumero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItems_OfficeId",
                table: "InventarioItems",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItems_SetorId",
                table: "InventarioItems",
                column: "SetorId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioItems_SistemaOperacionalId",
                table: "InventarioItems",
                column: "SistemaOperacionalId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioMemorias_Descricao",
                table: "InventarioMemorias",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioMonitores_InventarioNumero",
                table: "InventarioMonitores",
                column: "InventarioNumero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioOffices_Nome",
                table: "InventarioOffices",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioPerifericos_Descricao",
                table: "InventarioPerifericos",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioProcessadores_Descricao",
                table: "InventarioProcessadores",
                column: "Descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioSetores_Nome",
                table: "InventarioSetores",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioSistemasOperacionais_Nome",
                table: "InventarioSistemasOperacionais",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministrativoUsuarios");

            migrationBuilder.DropTable(
                name: "Chamados");

            migrationBuilder.DropTable(
                name: "InventarioItemArmazenamentos");

            migrationBuilder.DropTable(
                name: "InventarioItemArmazenamentosQuantidades");

            migrationBuilder.DropTable(
                name: "InventarioItemCabos");

            migrationBuilder.DropTable(
                name: "InventarioItemCabosQuantidades");

            migrationBuilder.DropTable(
                name: "InventarioItemChavesLicencas");

            migrationBuilder.DropTable(
                name: "InventarioItemMemorias");

            migrationBuilder.DropTable(
                name: "InventarioItemMemoriasQuantidades");

            migrationBuilder.DropTable(
                name: "InventarioItemMonitores");

            migrationBuilder.DropTable(
                name: "InventarioItemPerifericos");

            migrationBuilder.DropTable(
                name: "InventarioItemPerifericosQuantidades");

            migrationBuilder.DropTable(
                name: "InventarioItemProcessadores");

            migrationBuilder.DropTable(
                name: "InventarioArmazenamentos");

            migrationBuilder.DropTable(
                name: "InventarioCabos");

            migrationBuilder.DropTable(
                name: "InventarioChavesLicencas");

            migrationBuilder.DropTable(
                name: "InventarioMemorias");

            migrationBuilder.DropTable(
                name: "InventarioMonitores");

            migrationBuilder.DropTable(
                name: "InventarioPerifericos");

            migrationBuilder.DropTable(
                name: "InventarioItems");

            migrationBuilder.DropTable(
                name: "InventarioProcessadores");

            migrationBuilder.DropTable(
                name: "InventarioAntivirus");

            migrationBuilder.DropTable(
                name: "InventarioConexoes");

            migrationBuilder.DropTable(
                name: "InventarioOffices");

            migrationBuilder.DropTable(
                name: "InventarioSetores");

            migrationBuilder.DropTable(
                name: "InventarioSistemasOperacionais");
        }
    }
}
