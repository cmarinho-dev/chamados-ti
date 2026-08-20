using System.Text.RegularExpressions;
using ChamadosTI.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ChamadosTI.Data;

public class ContextoChamados : DbContext
{
    public ContextoChamados(DbContextOptions<ContextoChamados> options)
        : base(options)
    {
    }

    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<InventarioItem> InventarioItems => Set<InventarioItem>();
    public DbSet<InventarioMemoria> InventarioMemorias => Set<InventarioMemoria>();
    public DbSet<InventarioProcessador> InventarioProcessadores => Set<InventarioProcessador>();
    public DbSet<InventarioArmazenamento> InventarioArmazenamentos => Set<InventarioArmazenamento>();
    public DbSet<InventarioMonitor> InventarioMonitores => Set<InventarioMonitor>();
    public DbSet<InventarioPeriferico> InventarioPerifericos => Set<InventarioPeriferico>();
    public DbSet<InventarioCabo> InventarioCabos => Set<InventarioCabo>();
    public DbSet<InventarioItemMemoriaQuantidade> InventarioItemMemoriasQuantidades => Set<InventarioItemMemoriaQuantidade>();
    public DbSet<InventarioItemArmazenamentoQuantidade> InventarioItemArmazenamentosQuantidades => Set<InventarioItemArmazenamentoQuantidade>();
    public DbSet<InventarioItemPerifericoQuantidade> InventarioItemPerifericosQuantidades => Set<InventarioItemPerifericoQuantidade>();
    public DbSet<InventarioItemCaboQuantidade> InventarioItemCabosQuantidades => Set<InventarioItemCaboQuantidade>();
    public DbSet<AdministrativoUsuario> AdministrativoUsuarios => Set<AdministrativoUsuario>();
    public DbSet<InventarioSetor> InventarioSetores => Set<InventarioSetor>();
    public DbSet<InventarioSistemaOperacional> InventarioSistemasOperacionais => Set<InventarioSistemaOperacional>();
    public DbSet<InventarioOffice> InventarioOffices => Set<InventarioOffice>();
    public DbSet<InventarioAntivirus> InventarioAntivirus => Set<InventarioAntivirus>();
    public DbSet<InventarioConexao> InventarioConexoes => Set<InventarioConexao>();
    public DbSet<InventarioChaveLicenca> InventarioChavesLicencas => Set<InventarioChaveLicenca>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventarioSetor>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<InventarioSistemaOperacional>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<InventarioOffice>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<InventarioAntivirus>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<InventarioConexao>()
            .HasIndex(s => s.Nome)
            .IsUnique();

        modelBuilder.Entity<AdministrativoUsuario>()
            .HasIndex(s => s.Usuario)
            .IsUnique();

        modelBuilder.Entity<InventarioChaveLicenca>()
            .HasIndex(s => new { s.Tipo, s.Chave })
            .IsUnique();

        modelBuilder.Entity<InventarioMemoria>()
            .HasIndex(s => s.Descricao)
            .IsUnique();

        modelBuilder.Entity<InventarioProcessador>()
            .HasIndex(s => s.Descricao)
            .IsUnique();

        modelBuilder.Entity<InventarioArmazenamento>()
            .HasIndex(s => s.Descricao)
            .IsUnique();

        modelBuilder.Entity<InventarioPeriferico>()
            .HasIndex(s => s.Descricao)
            .IsUnique();

        modelBuilder.Entity<InventarioCabo>()
            .HasIndex(s => s.Descricao)
            .IsUnique();

        modelBuilder.Entity<InventarioMonitor>()
            .HasIndex(s => s.InventarioNumero)
            .IsUnique();

        modelBuilder.Entity<InventarioItem>()
            .HasIndex(a => a.InventarioNumero)
            .IsUnique();

        modelBuilder.Entity<InventarioItem>()
            .HasOne(a => a.Setor)
            .WithMany()
            .HasForeignKey(a => a.SetorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventarioItem>()
            .HasOne(a => a.SistemaOperacional)
            .WithMany()
            .HasForeignKey(a => a.SistemaOperacionalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventarioItem>()
            .HasOne(a => a.Office)
            .WithMany()
            .HasForeignKey(a => a.OfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventarioItem>()
            .HasOne(a => a.Antivirus)
            .WithMany()
            .HasForeignKey(a => a.AntivirusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventarioItem>()
            .HasOne(a => a.Conexao)
            .WithMany()
            .HasForeignKey(a => a.ConexaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdministrativoUsuario>()
            .HasOne(a => a.Departamento)
            .WithMany()
            .HasForeignKey(a => a.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventarioItem>()
            .HasMany(i => i.Processadores)
            .WithMany(c => c.InventarioItems)
            .UsingEntity<Dictionary<string, object>>("inventarioitemprocessadores");

        modelBuilder.Entity<InventarioItem>()
            .HasMany(i => i.Monitores)
            .WithMany(c => c.InventarioItems)
            .UsingEntity<Dictionary<string, object>>("inventarioitemmonitores");


        modelBuilder.Entity<InventarioItemMemoriaQuantidade>()
            .HasKey(x => new { x.InventarioItemId, x.MemoriaId });

        modelBuilder.Entity<InventarioItemMemoriaQuantidade>()
            .HasOne(x => x.InventarioItem)
            .WithMany(i => i.MemoriasQuantidades)
            .HasForeignKey(x => x.InventarioItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemMemoriaQuantidade>()
            .HasOne(x => x.Memoria)
            .WithMany()
            .HasForeignKey(x => x.MemoriaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemArmazenamentoQuantidade>()
            .HasKey(x => new { x.InventarioItemId, x.ArmazenamentoId });

        modelBuilder.Entity<InventarioItemArmazenamentoQuantidade>()
            .HasOne(x => x.InventarioItem)
            .WithMany(i => i.ArmazenamentosQuantidades)
            .HasForeignKey(x => x.InventarioItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemArmazenamentoQuantidade>()
            .HasOne(x => x.Armazenamento)
            .WithMany()
            .HasForeignKey(x => x.ArmazenamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemPerifericoQuantidade>()
            .HasKey(x => new { x.InventarioItemId, x.PerifericoId });

        modelBuilder.Entity<InventarioItemPerifericoQuantidade>()
            .HasOne(x => x.InventarioItem)
            .WithMany(i => i.PerifericosQuantidades)
            .HasForeignKey(x => x.InventarioItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemPerifericoQuantidade>()
            .HasOne(x => x.Periferico)
            .WithMany()
            .HasForeignKey(x => x.PerifericoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemCaboQuantidade>()
            .HasKey(x => new { x.InventarioItemId, x.CaboId });

        modelBuilder.Entity<InventarioItemCaboQuantidade>()
            .HasOne(x => x.InventarioItem)
            .WithMany(i => i.CabosQuantidades)
            .HasForeignKey(x => x.InventarioItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItemCaboQuantidade>()
            .HasOne(x => x.Cabo)
            .WithMany()
            .HasForeignKey(x => x.CaboId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioItem>()
            .HasMany(i => i.ChavesLicencas)
            .WithMany(c => c.InventarioItems)
            .UsingEntity<Dictionary<string, object>>("inventarioitemchaveslicencas");

        modelBuilder.Entity<InventarioSetor>().HasData(
            new InventarioSetor { Id = 1, Nome = "PRESIDENCIA / GABINETE" },
            new InventarioSetor { Id = 2, Nome = "ASSESSORIA DE COMUNICACAO" },
            new InventarioSetor { Id = 3, Nome = "ASSESSORIA TECNICA" },
            new InventarioSetor { Id = 4, Nome = "APAF" },
            new InventarioSetor { Id = 5, Nome = "APDI" },
            new InventarioSetor { Id = 6, Nome = "APPLI" },
            new InventarioSetor { Id = 7, Nome = "BIBLIOTECA" },
            new InventarioSetor { Id = 8, Nome = "CENTRO DE EVENTOS IMAP BARIGUI" },
            new InventarioSetor { Id = 9, Nome = "EAP" },
            new InventarioSetor { Id = 10, Nome = "ESTAGIO" },
            new InventarioSetor { Id = 11, Nome = "ESTUDIO" },
            new InventarioSetor { Id = 12, Nome = "NEAD" },
            new InventarioSetor { Id = 13, Nome = "NIT" },
            new InventarioSetor { Id = 14, Nome = "SEGURO" },
            new InventarioSetor { Id = 15, Nome = "RH" }
        );

        modelBuilder.Entity<InventarioAntivirus>().HasData(
            new InventarioAntivirus { Id = 1, Nome = "Kaspersky" },
            new InventarioAntivirus { Id = 2, Nome = "N/T" }
        );

        modelBuilder.Entity<InventarioConexao>().HasData(
            new InventarioConexao { Id = 1, Nome = "Ethernet" },
            new InventarioConexao { Id = 2, Nome = "Wifi" },
            new InventarioConexao { Id = 3, Nome = "N/T" }
        );

        modelBuilder.Entity<InventarioOffice>().HasData(
            new InventarioOffice { Id = 1, Nome = "2007" },
            new InventarioOffice { Id = 2, Nome = "2010" },
            new InventarioOffice { Id = 3, Nome = "2013" },
            new InventarioOffice { Id = 4, Nome = "2016" },
            new InventarioOffice { Id = 5, Nome = "2019" },
            new InventarioOffice { Id = 6, Nome = "365" },
            new InventarioOffice { Id = 7, Nome = "N/T" },
            new InventarioOffice { Id = 8, Nome = "MacOS" }
        );

        modelBuilder.Entity<InventarioSistemaOperacional>().HasData(
            new InventarioSistemaOperacional { Id = 1, Nome = "Windows XP" },
            new InventarioSistemaOperacional { Id = 2, Nome = "Windows 7" },
            new InventarioSistemaOperacional { Id = 3, Nome = "Windows 8" },
            new InventarioSistemaOperacional { Id = 4, Nome = "Windows 8.1" },
            new InventarioSistemaOperacional { Id = 5, Nome = "Windows 10" },
            new InventarioSistemaOperacional { Id = 6, Nome = "Windows 11" },
            new InventarioSistemaOperacional { Id = 7, Nome = "MacOS" },
            new InventarioSistemaOperacional { Id = 8, Nome = "Arlequim/Ubuntu" }

        );

        modelBuilder.Entity<InventarioMemoria>().HasData(
            new InventarioMemoria { Id = 1, Descricao = "DDR2 2GB" },
            new InventarioMemoria { Id = 2, Descricao = "DDR2 4GB" },
            new InventarioMemoria { Id = 3, Descricao = "DDR2 8GB" },
            new InventarioMemoria { Id = 4, Descricao = "DDR3 4GB" },
            new InventarioMemoria { Id = 5, Descricao = "DDR3 8GB" },
            new InventarioMemoria { Id = 6, Descricao = "DDR3 16GB" },
            new InventarioMemoria { Id = 7, Descricao = "DDR4 4GB" },
            new InventarioMemoria { Id = 8, Descricao = "DDR4 8GB" },
            new InventarioMemoria { Id = 9, Descricao = "DDR4 16GB" },
            new InventarioMemoria { Id = 10, Descricao = "DDR4 32GB" },
            new InventarioMemoria { Id = 11, Descricao = "DDR4 64GB" }
        );

        modelBuilder.Entity<InventarioProcessador>().HasData(
            new InventarioProcessador { Id = 1, Descricao = "Intel Core i3 2th" },
            new InventarioProcessador { Id = 2, Descricao = "Intel Core i3 3th" },
            new InventarioProcessador { Id = 3, Descricao = "Intel Core i3 4th" },
            new InventarioProcessador { Id = 4, Descricao = "Intel Core i3 5th" },
            new InventarioProcessador { Id = 5, Descricao = "Intel Core i3 6th" },
            new InventarioProcessador { Id = 6, Descricao = "Intel Core i3 7th" },
            new InventarioProcessador { Id = 7, Descricao = "Intel Core i3 8th" },
            new InventarioProcessador { Id = 8, Descricao = "Intel Core i3 9th" },
            new InventarioProcessador { Id = 9, Descricao = "Intel Core i3 10th" },
            new InventarioProcessador { Id = 10, Descricao = "Intel Core i3 11th" },
            new InventarioProcessador { Id = 11, Descricao = "Intel Core i5 5th" },
            new InventarioProcessador { Id = 12, Descricao = "Intel Core i5 6th" },
            new InventarioProcessador { Id = 13, Descricao = "Intel Core i5 7th" },
            new InventarioProcessador { Id = 14, Descricao = "Intel Core i5 8th" },
            new InventarioProcessador { Id = 15, Descricao = "Intel Core i5 9th" },
            new InventarioProcessador { Id = 16, Descricao = "Intel Core i5 10th" },
            new InventarioProcessador { Id = 17, Descricao = "Intel Core i5 11th" },
            new InventarioProcessador { Id = 18, Descricao = "Intel Core i5 12th" },
            new InventarioProcessador { Id = 19, Descricao = "Intel Core i7 5th" },
            new InventarioProcessador { Id = 20, Descricao = "Intel Core i7 6th" },
            new InventarioProcessador { Id = 21, Descricao = "Intel Core i7 7th" },
            new InventarioProcessador { Id = 22, Descricao = "Intel Core i7 8th" },
            new InventarioProcessador { Id = 23, Descricao = "Intel Core i7 9th" },
            new InventarioProcessador { Id = 24, Descricao = "Intel Core i7 10th" },
            new InventarioProcessador { Id = 25, Descricao = "Intel Core i7 11th" },
            new InventarioProcessador { Id = 26, Descricao = "Intel Core i7 12th" }
        );

        modelBuilder.Entity<InventarioArmazenamento>().HasData(
            new InventarioArmazenamento { Id = 1, Descricao = "HDD 150GB" },
            new InventarioArmazenamento { Id = 2, Descricao = "HDD 300GB" },
            new InventarioArmazenamento { Id = 3, Descricao = "HDD 500GB" },
            new InventarioArmazenamento { Id = 4, Descricao = "HDD 1TB" },
            new InventarioArmazenamento { Id = 5, Descricao = "HDD 2TB" },
            new InventarioArmazenamento { Id = 6, Descricao = "SSD SATA 120GB" },
            new InventarioArmazenamento { Id = 7, Descricao = "SSD SATA 256GB" },
            new InventarioArmazenamento { Id = 8, Descricao = "SSD SATA 512GB" },
            new InventarioArmazenamento { Id = 9, Descricao = "SSD SATA 1TB" },
            new InventarioArmazenamento { Id = 10, Descricao = "SSD NVME 120GB" },
            new InventarioArmazenamento { Id = 11, Descricao = "SSD NVME 256GB" },
            new InventarioArmazenamento { Id = 12, Descricao = "SSD NVME 512GB" },
            new InventarioArmazenamento { Id = 13, Descricao = "SSD NVME 1TB" }
        );

        modelBuilder.Entity<InventarioPeriferico>().HasData(
            new InventarioPeriferico { Id = 1, Descricao = "Teclado Positivo" },
            new InventarioPeriferico { Id = 2, Descricao = "Mouse Positivo" },
            new InventarioPeriferico { Id = 3, Descricao = "Teclado Multilaser" },
            new InventarioPeriferico { Id = 4, Descricao = "Mouse Multilaser" },
            new InventarioPeriferico { Id = 5, Descricao = "Teclado Logitech" },
            new InventarioPeriferico { Id = 6, Descricao = "Mouse Logitech" },
            new InventarioPeriferico { Id = 7, Descricao = "Webcam Logitech C270" },
            new InventarioPeriferico { Id = 8, Descricao = "Webcam Logitech C920" }
        );

        modelBuilder.Entity<InventarioCabo>().HasData(
            new InventarioCabo { Id = 1, Descricao = "HDMI" },
            new InventarioCabo { Id = 2, Descricao = "VGA" },
            new InventarioCabo { Id = 3, Descricao = "DVI" },
            new InventarioCabo { Id = 4, Descricao = "Alimentacao EU - 3 pinos" },
            new InventarioCabo { Id = 5, Descricao = "Alimentacao EU - 2 pinos" },
            new InventarioCabo { Id = 6, Descricao = "Alimentacao US - 3 pinos" },
            new InventarioCabo { Id = 7, Descricao = "Alimentacao US - 2 pinos" }
            );

        /*    modelBuilder.Entity<InventarioMonitor>().HasData(
            new InventarioMonitor
            {
                Id = 1,
                InventarioNumero = "MON-0001",
                Patrimonio = "MON-EX-001",
                Marca = "LG",
                Modelo = "24MP400",
                Polegadas = "24",
                Observacao = "Monitor exemplo",
                CriadoEm = new DateTimeOffset(2026, 2, 22, 0, 0, 0, TimeSpan.Zero)
            }
        );

        modelBuilder.Entity<InventarioChaveLicenca>().HasData(
            new InventarioChaveLicenca
            {
                Id = 1,
                Tipo = InventarioChaveTipo.Windows,
                Produto = "Windows 11 Pro",
                Chave = "AAAAA-BBBBB-CCCCC-DDDDD-EEEEE",
                Observacao = "Chave exemplo",
                CriadoEm = new DateTimeOffset(2026, 2, 22, 0, 0, 0, TimeSpan.Zero)
            }
        );

        modelBuilder.Entity<InventarioItem>().HasData(
            new InventarioItem
            {
                Id = 1,
                InventarioNumero = "CPU-0001",
                Patrimonio = "CPU-EX-001",
                TipoEquipamento = InventarioTipoEquipamento.Computador,
                EhBackup = false,
                PessoaResponsavel = "Exemplo NIT",
                SetorId = 13,
                SistemaOperacionalId = 5,
                OfficeId = 7,
                AntivirusId = 1,
                ConexaoId = 1,
                Ip = "192.168.0.10",
                Observacao = "Computador exemplo com monitor e chave",
                CriadoEm = new DateTimeOffset(2026, 2, 22, 0, 0, 0, TimeSpan.Zero)
            }
        );

        modelBuilder.Entity("InventarioItemMemorias").HasData(
          new { InventarioItemsId = 1, MemoriasId = 4 }
        );

        modelBuilder.Entity("InventarioItemProcessadores").HasData(
            new { InventarioItemsId = 1, ProcessadoresId = 16 }
        );

        modelBuilder.Entity("InventarioItemArmazenamentos").HasData(
            new { InventarioItemsId = 1, ArmazenamentosId = 7 }
        );

        modelBuilder.Entity("InventarioItemPerifericos").HasData(
            new { InventarioItemsId = 1, PerifericosId = 1 },
            new { InventarioItemsId = 1, PerifericosId = 2 }
        );

        modelBuilder.Entity("InventarioItemCabos").HasData(
            new { InventarioItemsId = 1, CabosId = 1 },
            new { InventarioItemsId = 1, CabosId = 4 }
        );

        modelBuilder.Entity("InventarioItemMonitores").HasData(
            new { InventarioItemsId = 1, MonitoresId = 1 }
        );

        modelBuilder.Entity("InventarioItemChavesLicencas").HasData(
            new { InventarioItemsId = 1, ChavesLicencasId = 1 }
        );

        modelBuilder.Entity<InventarioItemMemoriaQuantidade>().HasData(
            new { InventarioItemId = 1, MemoriaId = 4, Quantidade = 2 }
        );

        modelBuilder.Entity<InventarioItemArmazenamentoQuantidade>().HasData(
            new { InventarioItemId = 1, ArmazenamentoId = 7, Quantidade = 1 }
        );

        modelBuilder.Entity<InventarioItemPerifericoQuantidade>().HasData(
            new { InventarioItemId = 1, PerifericoId = 1, Quantidade = 1 },
            new { InventarioItemId = 1, PerifericoId = 2, Quantidade = 1 }
        );

        modelBuilder.Entity<InventarioItemCaboQuantidade>().HasData(
            new { InventarioItemId = 1, CaboId = 1, Quantidade = 1 },
            new { InventarioItemId = 1, CaboId = 4, Quantidade = 1 }
        );

        modelBuilder.Entity<AdministrativoUsuario>().HasData(
            new AdministrativoUsuario
            {
                Id = 264,
                Nome = "Guilherme do Nascimento Sell",
                Usuario = "gsell",
                DepartamentoId = 13
            }
        ); */
    }

    /*
    // ============================================================
    // MÉTODO DE IMPORTAÇÃO DOS DADOS LEGADOS
    // ============================================================
    public async Task ImportarDadosLegadoAsync()
    {
        var connectionStringLegado = "server=localhost;port=3307;uid=root;pwd=qwert321;database=chamadostiold;";
        // Dicionários de mapeamento para lookup tables (cache)
        var setores = await InventarioSetores.ToDictionaryAsync(s => s.Nome, s => s.Id);
        var sistemasOp = await InventarioSistemasOperacionais.ToDictionaryAsync(s => s.Nome, s => s.Id);
        var offices = await InventarioOffices.ToDictionaryAsync(o => o.Nome, o => o.Id);
        var antivirus = await InventarioAntivirus.ToDictionaryAsync(a => a.Nome, a => a.Id);
        var conexoes = await InventarioConexoes.ToDictionaryAsync(c => c.Nome, c => c.Id);
        var memorias = await InventarioMemorias.ToDictionaryAsync(m => m.Descricao, m => m.Id);
        var processadores = await InventarioProcessadores.ToDictionaryAsync(p => p.Descricao, p => p.Id);
        var armazenamentos = await InventarioArmazenamentos.ToDictionaryAsync(a => a.Descricao, a => a.Id);
        // var monitoresExistentes = new HashSet<string>(await InventarioMonitores.Select(m => m.InventarioNumero).ToListAsync());

        // Cache para monitores (evita consultas repetidas e problemas com FirstAsync)
        var monitoresCache = new Dictionary<string, InventarioMonitor>(StringComparer.OrdinalIgnoreCase);

        // Cache para chaves de licença (evita SaveChanges a cada chave)
        var chavesCache = new Dictionary<(InventarioChaveTipo Tipo, string Chave), InventarioChaveLicenca>();

        // Mapeamento de departamentos antigos para setores novos (baseado nos nomes)
        var departamentoParaSetor = new Dictionary<int, int>
    {
        // Baseado na tabela 'departamentos' do banco antigo
        {1, setores["PRESIDENCIA / GABINETE"]},   // PRESIDÊNCIA
        {2, setores["PRESIDENCIA / GABINETE"]},          // SUPERINTENDÊNCIA
        {3, setores["ASSESSORIA TECNICA"]},
        {4, setores["APAF"]},
        {6, setores["APDI"]},
        {7, setores["EAP"]},
        {8, setores["CENTRO DE EVENTOS IMAP BARIGUI"]}, // BARIGUI
        {15, setores["NIT"]},
        {16, setores["ASSESSORIA DE COMUNICACAO"]},
        {18, setores["CENTRO DE EVENTOS IMAP BARIGUI"]},
        {21, setores["APDI"]}, // APPA (mapeado para APDI, ajuste se necessário)
        {22, setores["APPLI"]},
        {23, setores["NEAD"]},
        {24, setores["ESTAGIO"]},
        {25, setores["PRESIDENCIA / GABINETE"]}, // GABINETE
        {27, setores["SEGURO"]},
        {28, setores["BIBLIOTECA"]},
        {30, setores["CENTRO DE EVENTOS IMAP BARIGUI"]}, // EXTERNO
        {31, setores["CENTRO DE EVENTOS IMAP BARIGUI"]}, // BACKUP
        {32, setores["RH"]},
        {33, setores["CENTRO DE EVENTOS IMAP BARIGUI"]}, // BACKUP-BARIGUI
        {34, setores["CENTRO DE EVENTOS IMAP BARIGUI"]}, // BARIGUI (outro)
        {35, setores["ESTUDIO"]},
        {36, setores["CENTRO DE EVENTOS IMAP BARIGUI"]}  // WORKTIBA
    };

        // --------------------------------------------------------
        // 1. IMPORTAR CLIENTES -> AdministrativoUsuarios
        // --------------------------------------------------------
        using var connLegado = new MySqlConnection(connectionStringLegado);
        await connLegado.OpenAsync();

        // Buscar clientes (tabela 'clientes')
        var clientes = new List<ClienteAntigo>();
        using (var cmd = new MySqlCommand("SELECT SOLI_CODIGO, SOLI_NOME, SOLI_LOGIN, DEPA_CODIGO FROM clientes", connLegado))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            int i = 1;
            int i2 = 1;
            int i3 = 1;
            while (await reader.ReadAsync())
            {
                switch (reader.GetInt32(0))
                {
                    case 398:
                        clientes.Add(new ClienteAntigo
                        {
                            SOLI_CODIGO = reader.GetInt32(0),
                            SOLI_NOME = reader.GetString(1),
                            SOLI_LOGIN = "RESERVA_BCKP",
                            DEPA_CODIGO = reader.GetInt32(3)
                        });
                        break;
                    case 401:
                        clientes.Add(new ClienteAntigo
                        {
                            SOLI_CODIGO = reader.GetInt32(0),
                            SOLI_NOME = reader.GetString(1),
                            SOLI_LOGIN = "RESERVA_BARIGUI",
                            DEPA_CODIGO = reader.GetInt32(3)
                        });
                        break;
                    case 405:
                        clientes.Add(new ClienteAntigo
                        {
                            SOLI_CODIGO = reader.GetInt32(0),
                            SOLI_NOME = reader.GetString(1),
                            SOLI_LOGIN = "RESERVA_ESTAGIO",
                            DEPA_CODIGO = reader.GetInt32(3)
                        });
                        break;
                    case 472:
                        clientes.Add(new ClienteAntigo
                        {
                            SOLI_CODIGO = reader.GetInt32(0),
                            SOLI_NOME = reader.GetString(1),
                            SOLI_LOGIN = "RESERVA_DIRETPLANEINOVAC",
                            DEPA_CODIGO = reader.GetInt32(3)
                        });
                        break;
                    default:
                        if (reader.GetString(2) == "s/n" || reader.GetString(2) == "s/l" || reader.GetString(2) == "S/l")
                        {
                            clientes.Add(new ClienteAntigo
                            {
                                SOLI_CODIGO = reader.GetInt32(0),
                                SOLI_NOME = reader.GetString(1),
                                SOLI_LOGIN = reader.GetString(1).Replace(" ", "") + "_usr",
                                DEPA_CODIGO = reader.GetInt32(3)
                            });
                        }
                        else if (reader.GetString(2).Contains("Sem Usu"))
                        {
                            clientes.Add(new ClienteAntigo
                            {
                                SOLI_CODIGO = reader.GetInt32(0),
                                SOLI_NOME = reader.GetString(1),
                                SOLI_LOGIN = reader.GetString(1).Replace(" ", "") + "_usr" + i,
                                DEPA_CODIGO = reader.GetInt32(3)
                            });
                            i++;
                        }
                        else if (reader.GetString(2) == "")
                        {
                            clientes.Add(new ClienteAntigo
                            {
                                SOLI_CODIGO = reader.GetInt32(0),
                                SOLI_NOME = reader.GetString(1),
                                SOLI_LOGIN = reader.GetString(1) + i2,
                                DEPA_CODIGO = reader.GetInt32(3)
                            });
                            i2++;
                        }
                        else if (reader.GetString(2) == "gzapata")
                        {
                            clientes.Add(new ClienteAntigo
                            {
                                SOLI_CODIGO = reader.GetInt32(0),
                                SOLI_NOME = reader.GetString(1),
                                SOLI_LOGIN = reader.GetString(2) + i3,
                                DEPA_CODIGO = reader.GetInt32(3)
                            });
                            i3++;
                        }
                        else
                        {
                            clientes.Add(new ClienteAntigo
                            {
                                SOLI_CODIGO = reader.GetInt32(0),
                                SOLI_NOME = reader.GetString(1),
                                SOLI_LOGIN = reader.IsDBNull(2) ? null : reader.GetString(2),
                                DEPA_CODIGO = reader.GetInt32(3)
                            });
                        }
                        break;
                }
            }
        }

        foreach (var cli in clientes)
        {
            // Se o login for nulo ou vazio, pode gerar conflito de unique; definimos um padrão
            string usuario = string.IsNullOrWhiteSpace(cli.SOLI_LOGIN) ? $"user_{cli.SOLI_CODIGO}" : cli.SOLI_LOGIN;

            if (!await AdministrativoUsuarios.AnyAsync(u => u.Id == cli.SOLI_CODIGO))
            {
                var novoUsuario = new AdministrativoUsuario
                {
                    Id = cli.SOLI_CODIGO,
                    Nome = cli.SOLI_NOME,
                    Usuario = usuario,
                    DepartamentoId = departamentoParaSetor.ContainsKey(cli.DEPA_CODIGO)
                        ? departamentoParaSetor[cli.DEPA_CODIGO]
                        : setores["CENTRO DE EVENTOS IMAP BARIGUI"] // fallback
                };
                AdministrativoUsuarios.Add(novoUsuario);
            }
        }
        await SaveChangesAsync();

        // --------------------------------------------------------
        // 2. IMPORTAR INVENTARIO (computadores) -> InventarioItem
        // --------------------------------------------------------
        var inventarioItens = new List<InventarioItem>();
        using (var cmd = new MySqlCommand("SELECT * FROM inventario", connLegado))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var item = new InventarioItem
                {
                    InventarioNumero = reader["INV_INVENTARIO"]?.ToString() ?? Guid.NewGuid().ToString(),
                    Patrimonio = reader["INV_PATRIMONIO"]?.ToString(),
                    TipoEquipamento = InventarioTipoEquipamento.Computador,
                    EhBackup = reader["INV_SETOR"]?.ToString()?.Contains("BACKUP") == true ||
                               reader["INV_USUARIO"]?.ToString() == "RESERVA",
                    PessoaResponsavel = reader["INV_USUARIO"]?.ToString(),
                    SetorId = ObterIdSetor(reader["INV_SETOR"]?.ToString(), setores),
                    SistemaOperacionalId = ObterIdSO(reader["INV_WINDOWS"]?.ToString(), sistemasOp),
                    OfficeId = ObterIdOffice(reader["INV_OFFICE"]?.ToString(), offices),
                    AntivirusId = reader["INV_ANTIVIRUS"]?.ToString() == "KASPERSKY" ? 1 : 2, // N/T
                    ConexaoId = ObterIdConexao(reader["INV_CONEXAO"]?.ToString(), conexoes),
                    Ip = reader["INV_IP"]?.ToString(),
                    Observacao = MontarObservacao(reader),
                    CriadoEm = reader["INV_DATA_MODIFICACAO"] as DateTime? ?? DateTime.Now
                };

                // Memória RAM
                string memoriaStr = reader["INV_MEMORIA_RAM"]?.ToString();
                if (!string.IsNullOrWhiteSpace(memoriaStr))
                {
                    int memoriaId = ObterIdMemoria(memoriaStr, memorias);
                    item.MemoriasQuantidades.Add(new InventarioItemMemoriaQuantidade
                    {
                        MemoriaId = memoriaId,
                        Quantidade = 1
                    });
                }

                // Processador
                string procStr = reader["INV_PROCESSADOR"]?.ToString();
                string geracao = reader["INV_GERACAO"]?.ToString();
                if (!string.IsNullOrWhiteSpace(procStr))
                {
                    string descProc = $"{procStr} {geracao}".Replace("°", "th").Trim();
                    int procId = ObterIdProcessador(descProc, processadores);
                    // Adiciona na relação muitos-para-muitos sem quantidade
                    var proc = await InventarioProcessadores.FindAsync(procId);
                    if (proc != null) item.Processadores.Add(proc);
                }

                // Armazenamento (HD)
                string hdStr = reader["INV_HD"]?.ToString();
                foreach (var hd in ParseHD(hdStr))
                {
                    int armId = ObterIdArmazenamento(hd, armazenamentos);
                    item.ArmazenamentosQuantidades.Add(new InventarioItemArmazenamentoQuantidade
                    {
                        ArmazenamentoId = armId,
                        Quantidade = 1
                    });
                }

                // Monitores
                for (int i = 1; i <= 2; i++)
                {
                    string invMonitor = reader[$"INV_INVENTARIO_MONITOR_{i}"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(invMonitor) && invMonitor != "N/T")
                    {
                        if (!monitoresCache.TryGetValue(invMonitor, out var monitor))
                        {
                            // Tenta buscar no banco (pode existir de importações anteriores)
                            monitor = await InventarioMonitores.FirstOrDefaultAsync(m => m.InventarioNumero == invMonitor);
                            if (monitor == null)
                            {
                                string marca = reader[$"INV_MARCA_MONITOR_{i}"]?.ToString();
                                string pol = reader[$"INV_POLEGADAS_MONITOR_{i}"]?.ToString();
                                monitor = new InventarioMonitor
                                {
                                    InventarioNumero = invMonitor,
                                    Marca = marca,
                                    Polegadas = pol,
                                    Observacao = $"Monitor do setor {reader["INV_SETOR"]}",
                                    CriadoEm = DateTime.Now
                                };
                                InventarioMonitores.Add(monitor);
                            }
                            monitoresCache[invMonitor] = monitor;
                        }
                        item.Monitores.Add(monitor);
                    }
                }
                // No loop de inventário, ao processar monitores:
                for (int i = 1; i <= 2; i++)
                {
                    string invMonitor = reader[$"INV_INVENTARIO_MONITOR_{i}"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(invMonitor) && invMonitor != "N/T")
                    {
                        // Verifica se já está no cache
                        if (!monitoresCache.TryGetValue(invMonitor, out var monitor))
                        {
                            // Tenta buscar no banco (caso já exista de importações anteriores ou seed)
                            monitor = await InventarioMonitores.FirstOrDefaultAsync(m => m.InventarioNumero == invMonitor);
                            if (monitor == null)
                            {
                                // Criar novo monitor
                                string marca = reader[$"INV_MARCA_MONITOR_{i}"]?.ToString();
                                string pol = reader[$"INV_POLEGADAS_MONITOR_{i}"]?.ToString();
                                monitor = new InventarioMonitor
                                {
                                    InventarioNumero = invMonitor,
                                    Marca = marca,
                                    Polegadas = pol,
                                    Observacao = $"Monitor do setor {reader["INV_SETOR"]}",
                                    CriadoEm = DateTime.Now
                                };
                                InventarioMonitores.Add(monitor);
                                // Após adicionar ao contexto, podemos salvar imediatamente para ter o ID,
                                // mas para performance, pode-se salvar em lote depois. Por enquanto, adicionamos ao cache.
                            }
                            monitoresCache[invMonitor] = monitor;
                        }
                        // Associa ao item
                        item.Monitores.Add(monitor);
                    }
                }

                // Chave do Office
                string chaveOffice = reader["INV_CHAVE_OFFICE"]?.ToString();
                if (!string.IsNullOrWhiteSpace(chaveOffice) && chaveOffice != "N/T")
                {
                    var chave = await ObterOuCriarChaveLicencaAsync(InventarioChaveTipo.Office, chaveOffice);
                    item.ChavesLicencas.Add(chave);
                }

                inventarioItens.Add(item);

                if (inventarioItens.Count % 100 == 0)
                {
                    InventarioItems.AddRange(inventarioItens);
                    await SaveChangesAsync();
                    inventarioItens.Clear();
                }
            }
        }
        if (inventarioItens.Any())
        {
            InventarioItems.AddRange(inventarioItens);
            await SaveChangesAsync();
        }

        // --------------------------------------------------------
        // 3. IMPORTAR NOTEBOOK -> InventarioItem
        // --------------------------------------------------------
        var notebooks = new List<InventarioItem>();
        using (var cmd = new MySqlCommand("SELECT * FROM notebook", connLegado))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var item = new InventarioItem
                {
                    InventarioNumero = reader["NOT_INVENTARIO"]?.ToString() ?? Guid.NewGuid().ToString(),
                    Patrimonio = reader["NOT_PATRIMONIO"]?.ToString(),
                    TipoEquipamento = InventarioTipoEquipamento.Notebook,
                    EhBackup = reader["NOT_SETOR"]?.ToString()?.Contains("BACKUP") == true,
                    PessoaResponsavel = reader["NOT_USUARIO"]?.ToString(),
                    SetorId = ObterIdSetor(reader["NOT_SETOR"]?.ToString(), setores),
                    SistemaOperacionalId = ObterIdSO(reader["NOT_WINDOWS"]?.ToString(), sistemasOp),
                    OfficeId = ObterIdOffice(reader["NOT_OFFICE"]?.ToString(), offices),
                    AntivirusId = 1, // Kaspersky (padrão)
                    ConexaoId = conexoes["Ethernet"], // por padrão
                    Observacao = reader["NOT_OBSERVACAO"]?.ToString(),
                    CriadoEm = reader["NOT_DATA_MODIFICACAO"] as DateTime? ?? DateTime.Now
                };

                // Processador
                string procStr = reader["NOT_PROCESSADOR"]?.ToString();
                string geracao = reader["NOT_GERACAO"]?.ToString();
                if (!string.IsNullOrWhiteSpace(procStr))
                {
                    string descProc = $"{procStr} {geracao}".Replace("°", "th").Trim();
                    int procId = ObterIdProcessador(descProc, processadores);
                    var proc = await InventarioProcessadores.FindAsync(procId);
                    if (proc != null) item.Processadores.Add(proc);
                }

                // Memória RAM
                string memoriaStr = reader["NOT_MEMORIA_RAM"]?.ToString();
                if (!string.IsNullOrWhiteSpace(memoriaStr))
                {
                    int memoriaId = ObterIdMemoria(memoriaStr, memorias);
                    item.MemoriasQuantidades.Add(new InventarioItemMemoriaQuantidade
                    {
                        MemoriaId = memoriaId,
                        Quantidade = 1
                    });
                }

                // Armazenamento
                string hdStr = reader["NOT_HD"]?.ToString();
                foreach (var hd in ParseHD(hdStr))
                {
                    int armId = ObterIdArmazenamento(hd, armazenamentos);
                    item.ArmazenamentosQuantidades.Add(new InventarioItemArmazenamentoQuantidade
                    {
                        ArmazenamentoId = armId,
                        Quantidade = 1
                    });
                }

                // Chave Windows
                string chaveWin = reader["NOT_CHAVE_WINDOWS"]?.ToString();
                if (!string.IsNullOrWhiteSpace(chaveWin))
                {
                    var chave = await ObterOuCriarChaveLicencaAsync(InventarioChaveTipo.Windows, chaveWin);
                    item.ChavesLicencas.Add(chave);
                }

                // Chave Office
                string chaveOff = reader["NOT_CHAVE_OFFICE"]?.ToString();
                if (!string.IsNullOrWhiteSpace(chaveOff))
                {
                    var chave = await ObterOuCriarChaveLicencaAsync(InventarioChaveTipo.Office, chaveOff);
                    item.ChavesLicencas.Add(chave);
                }

                notebooks.Add(item);

                if (notebooks.Count % 100 == 0)
                {
                    InventarioItems.AddRange(notebooks);
                    await SaveChangesAsync();
                    notebooks.Clear();
                }
            }
        }
        if (notebooks.Any())
        {
            InventarioItems.AddRange(notebooks);
            await SaveChangesAsync();
        }

        // --------------------------------------------------------
        // 4. IMPORTAR MONITORES AVULSOS (monitores e monitores_barigui)
        // --------------------------------------------------------
        // Tabela monitores
        using (var cmd = new MySqlCommand("SELECT * FROM monitores", connLegado))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                string invNum = reader["MON_INVENTARIO"]?.ToString();
                if (!string.IsNullOrWhiteSpace(invNum) && !monitoresCache.ContainsKey(invNum))
                {
                    var monitor = new InventarioMonitor
                    {
                        InventarioNumero = invNum,
                        Marca = reader["MON_MARCA"]?.ToString(),
                        Polegadas = reader["MON_POLEGADAS"]?.ToString(),
                        Observacao = $"Setor: {reader["MON_SETOR"]}",
                        CriadoEm = reader["MON_DATA_MODIFICACAO"] as DateTime? ?? DateTime.Now
                    };
                    InventarioMonitores.Add(monitor);
                    monitoresCache[invNum] = monitor;
                }
            }
        }
        // Tabela monitores_barigui
        using (var cmd = new MySqlCommand("SELECT * FROM monitores_barigui", connLegado))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                string invNum = reader["MON_INVENTARIO"]?.ToString();
                if (!string.IsNullOrWhiteSpace(invNum) && !monitoresCache.ContainsKey(invNum))
                {
                    var monitor = new InventarioMonitor
                    {
                        InventarioNumero = invNum,
                        Marca = reader["MON_MARCA"]?.ToString(),
                        Polegadas = reader["MON_POLEGADAS"]?.ToString(),
                        Observacao = $"Setor: {reader["MON_SETOR"]} - Barigui",
                        CriadoEm = reader["MON_DATA_MODIFICACAO"] as DateTime? ?? DateTime.Now
                    };
                    InventarioMonitores.Add(monitor);
                    monitoresCache[invNum] = monitor;
                }
            }
        }
        await SaveChangesAsync();
    }
    
    // Versão em cache do método ObterOuCriarChaveLicenca
    private async Task<InventarioChaveLicenca> ObterOuCriarChaveLicencaCachedAsync(
        InventarioChaveTipo tipo,
        string chave,
        Dictionary<(InventarioChaveTipo, string), InventarioChaveLicenca> cache)
    {
        var key = (tipo, chave);
        if (cache.TryGetValue(key, out var licenca))
            return licenca;

        licenca = await InventarioChavesLicencas.FirstOrDefaultAsync(l => l.Tipo == tipo && l.Chave == chave);
        if (licenca == null)
        {
            licenca = new InventarioChaveLicenca
            {
                Tipo = tipo,
                Produto = tipo == InventarioChaveTipo.Office ? "Microsoft Office" : "Microsoft Windows",
                Chave = chave,
                CriadoEm = DateTime.Now
            };
            // Não adiciona ao contexto ainda; faremos em lote no final
        }
        cache[key] = licenca;
        return licenca;
    }

    // ========== MÉTODOS AUXILIARES ==========

    private int ObterIdSetor(string nomeSetor, Dictionary<string, int> setores)
    {
        if (string.IsNullOrWhiteSpace(nomeSetor)) return setores["CENTRO DE EVENTOS IMAP BARIGUI"];
        // Tenta correspondência exata; se falhar, usa fallback
        return setores.TryGetValue(nomeSetor.ToUpperInvariant(), out int id) ? id : setores["CENTRO DE EVENTOS IMAP BARIGUI"];
    }

    private int? ObterIdSO(string so, Dictionary<string, int> sistemasOp)
    {
        if (string.IsNullOrWhiteSpace(so)) return null;
        // Normaliza: "WINDOWS 10" -> "Windows 10"
        string key = so.Replace("WINDOWS", "Windows").Replace("ARLEQUIM", "Arlequim/Ubuntu").Trim();
        return sistemasOp.TryGetValue(key, out int id) ? id : null;
    }

    private int? ObterIdOffice(string office, Dictionary<string, int> offices)
    {
        if (string.IsNullOrWhiteSpace(office) || office == "Não Tem") return offices["N/T"];
        return offices.TryGetValue(office, out int id) ? id : offices["N/T"];
    }

    private int? ObterIdConexao(string conexao, Dictionary<string, int> conexoes)
    {
        if (string.IsNullOrWhiteSpace(conexao)) return null;
        string key = conexao switch
        {
            "CABO DE REDE" => "Ethernet",
            "WIFI" => "Wifi",
            _ => "N/T"
        };
        return conexoes[key];
    }

    private int ObterIdMemoria(string memoria, Dictionary<string, int> memorias)
    {
        // Ex: "08 GB" -> "8 GB" + suposição de DDR
        string tamanho = memoria.Replace(" ", "").Replace("0", "").Trim(); // "8 GB"
                                                                           // Tenta encontrar correspondência exata; se não, tenta DDR4 primeiro, depois DDR3
        string[] tipos = { "DDR4", "DDR3", "DDR2" };

        int quantMemoria = int.Parse(Regex.Match(memoria, @"\d+").Value);

        foreach (var tipo in tipos)
        {
            string chave = $"{tipo} {tamanho}";
            if (memorias.ContainsKey(chave)) return memorias[chave];
        }
        // Fallback: DDR3/DDR4 (mais comum)
        if (quantMemoria > 8)
        {
            return memorias[$"DDR4 {tamanho}"];
        }
        return memorias[$"DDR3 {tamanho}"];
    }

    private int ObterIdProcessador(string descricao, Dictionary<string, int> processadores)
    {
        return processadores.TryGetValue(descricao, out int id) ? id : processadores.First().Value; // fallback
    }

    private int ObterIdArmazenamento(string descricao, Dictionary<string, int> armazenamentos)
    {
        // Tenta correspondência exata; se falhar, usa lógica de aproximação
        if (armazenamentos.TryGetValue(descricao, out int id)) return id;

        // Extrai tamanho e tipo
        string tamanho = "";
        if (descricao.Contains("150GB")) tamanho = "150GB";
        else if (descricao.Contains("300GB")) tamanho = "300GB";
        else if (descricao.Contains("500GB")) tamanho = "500GB";
        else if (descricao.Contains("1TB")) tamanho = "1TB";
        else if (descricao.Contains("2TB")) tamanho = "2TB";
        else if (descricao.Contains("256GB")) tamanho = "256GB";
        else if (descricao.Contains("512GB")) tamanho = "512GB";
        else if (descricao.Contains("120GB")) tamanho = "120GB";
        else tamanho = "500GB"; // fallback

        bool isSSD = descricao.Contains("SSD");
        bool isNVME = descricao.Contains("NVME");
        string tipo = isSSD ? (isNVME ? "NVME" : "SATA") : "HDD";

        string chave = tipo == "HDD" ? $"HDD {tamanho}" : (tipo == "NVME" ? $"SSD NVME {tamanho}" : $"SSD SATA {tamanho}");
        return armazenamentos.TryGetValue(chave, out int id2) ? id2 : armazenamentos.First().Value;
    }

    private IEnumerable<string> ParseHD(string hd)
    {
        if (string.IsNullOrWhiteSpace(hd)) yield break;
        foreach (var parte in hd.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            string limpo = parte.Trim();
            yield return limpo;
        }
    }

    private string MontarObservacao(MySqlDataReader reader)
    {
        var partes = new List<string>();
        if (!reader.IsDBNull(reader.GetOrdinal("INV_JUSTIFICATIVA")))
            partes.Add(reader.GetString("INV_JUSTIFICATIVA"));
        if (!reader.IsDBNull(reader.GetOrdinal("CHAM_ATENDENTE")))
            partes.Add($"Atendente: {reader.GetString("CHAM_ATENDENTE")}");
        return string.Join(" | ", partes);
    }

    private async Task<InventarioChaveLicenca> ObterOuCriarChaveLicencaAsync(InventarioChaveTipo tipo, string chave)
    {
        var licenca = await InventarioChavesLicencas.FirstOrDefaultAsync(l => l.Tipo == tipo && l.Chave == chave);
        if (licenca == null)
        {
            licenca = new InventarioChaveLicenca
            {
                Tipo = tipo,
                Produto = tipo == InventarioChaveTipo.Office ? "Microsoft Office" : "Microsoft Windows",
                Chave = chave,
                CriadoEm = DateTime.Now
            };
            InventarioChavesLicencas.Add(licenca);
            await SaveChangesAsync(); // salva imediatamente para ter o ID
        }
        return licenca;
    }

    // Classe auxiliar para leitura dos clientes antigos
    private class ClienteAntigo
    {
        public int SOLI_CODIGO { get; set; }
        public string SOLI_NOME { get; set; }
        public string SOLI_LOGIN { get; set; }
        public int DEPA_CODIGO { get; set; }
    }
    */
}
