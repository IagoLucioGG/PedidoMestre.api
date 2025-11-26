using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PedidoMestre.data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SenhaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeFantasia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.IdEmpresa);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    NmUsuario = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Variacoes",
                columns: table => new
                {
                    IdVariacao = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrecoExtra = table.Column<decimal>(type: "numeric(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variacoes", x => x.IdVariacao);
                });

            migrationBuilder.CreateTable(
                name: "Lojas",
                columns: table => new
                {
                    IdLoja = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    Endereco = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ConfigDelivery = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lojas", x => x.IdLoja);
                    table.ForeignKey(
                        name: "FK_Lojas_Empresas_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresas",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Adicionais",
                columns: table => new
                {
                    IdAdicional = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adicionais", x => x.IdAdicional);
                    table.ForeignKey(
                        name: "FK_Adicionais_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bairros",
                columns: table => new
                {
                    IdBairro = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TaxaEntrega = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bairros", x => x.IdBairro);
                    table.ForeignKey(
                        name: "FK_Bairros_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaixaFechamentos",
                columns: table => new
                {
                    IdFechamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    SaldoInicial = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    SaldoFinal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaFechamentos", x => x.IdFechamento);
                    table.ForeignKey(
                        name: "FK_CaixaFechamentos_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaixaFechamentos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaixaMovimentos",
                columns: table => new
                {
                    IdMovimento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaMovimentos", x => x.IdMovimento);
                    table.ForeignKey(
                        name: "FK_CaixaMovimentos_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaixaMovimentos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                    table.ForeignKey(
                        name: "FK_Categorias_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    IdEndereco = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCliente = table.Column<long>(type: "bigint", nullable: false),
                    IdBairro = table.Column<int>(type: "integer", nullable: false),
                    Logradouro = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Principal = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.IdEndereco);
                    table.ForeignKey(
                        name: "FK_Enderecos_Bairros_IdBairro",
                        column: x => x.IdBairro,
                        principalTable: "Bairros",
                        principalColumn: "IdBairro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enderecos_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    IdProduto = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCategoria = table.Column<int>(type: "integer", nullable: false),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrecoBase = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    TempoPreparoMin = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.IdProduto);
                    table.ForeignKey(
                        name: "FK_Produtos_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produtos_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    IdPedido = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    IdCliente = table.Column<long>(type: "bigint", nullable: false),
                    IdEndereco = table.Column<long>(type: "bigint", nullable: false),
                    IdEntregador = table.Column<int>(type: "integer", nullable: true),
                    Origem = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FormaPagamento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PagamentoStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TaxaEntrega = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Enderecos_IdEndereco",
                        column: x => x.IdEndereco,
                        principalTable: "Enderecos",
                        principalColumn: "IdEndereco",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios_IdEntregador",
                        column: x => x.IdEntregador,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoVariacoes",
                columns: table => new
                {
                    IdProduto = table.Column<long>(type: "bigint", nullable: false),
                    IdVariacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoVariacoes", x => new { x.IdProduto, x.IdVariacao });
                    table.ForeignKey(
                        name: "FK_ProdutoVariacoes_Produtos_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProdutoVariacoes_Variacoes_IdVariacao",
                        column: x => x.IdVariacao,
                        principalTable: "Variacoes",
                        principalColumn: "IdVariacao",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    IdAvaliacao = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<long>(type: "bigint", nullable: false),
                    NotaPedido = table.Column<int>(type: "integer", nullable: false),
                    NotaPlataforma = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.IdAvaliacao);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KdsLogs",
                columns: table => new
                {
                    IdLog = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<long>(type: "bigint", nullable: false),
                    IdItem = table.Column<long>(type: "bigint", nullable: true),
                    StatusAntigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StatusNovo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: true),
                    Origem = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Observacao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RegistradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KdsLogs", x => x.IdLog);
                    table.ForeignKey(
                        name: "FK_KdsLogs_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KdsLogs_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KdsPedidos",
                columns: table => new
                {
                    IdKdsPedido = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<long>(type: "bigint", nullable: false),
                    IdLoja = table.Column<int>(type: "integer", nullable: false),
                    StatusCozinha = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Prioridade = table.Column<int>(type: "integer", nullable: true),
                    TempoEstimadoPreparo = table.Column<int>(type: "integer", nullable: true),
                    TempoRealPreparo = table.Column<int>(type: "integer", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IniciadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinalizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KdsPedidos", x => x.IdKdsPedido);
                    table.ForeignKey(
                        name: "FK_KdsPedidos_Lojas_IdLoja",
                        column: x => x.IdLoja,
                        principalTable: "Lojas",
                        principalColumn: "IdLoja",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KdsPedidos_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItens",
                columns: table => new
                {
                    IdPedidoItem = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<long>(type: "bigint", nullable: false),
                    IdProduto = table.Column<long>(type: "bigint", nullable: false),
                    IdVariacao = table.Column<int>(type: "integer", nullable: true),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItens", x => x.IdPedidoItem);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Produtos_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoItens_Variacoes_IdVariacao",
                        column: x => x.IdVariacao,
                        principalTable: "Variacoes",
                        principalColumn: "IdVariacao",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KdsPedidoItens",
                columns: table => new
                {
                    IdKdsItem = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdKdsPedido = table.Column<long>(type: "bigint", nullable: false),
                    IdProduto = table.Column<long>(type: "bigint", nullable: false),
                    NomeProduto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Variacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Quantidade = table.Column<int>(type: "integer", nullable: true),
                    Observacao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StatusItem = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KdsPedidoItens", x => x.IdKdsItem);
                    table.ForeignKey(
                        name: "FK_KdsPedidoItens_KdsPedidos_IdKdsPedido",
                        column: x => x.IdKdsPedido,
                        principalTable: "KdsPedidos",
                        principalColumn: "IdKdsPedido",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KdsPedidoItens_Produtos_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItemAdicionais",
                columns: table => new
                {
                    IdPedidoItem = table.Column<long>(type: "bigint", nullable: false),
                    IdAdicional = table.Column<int>(type: "integer", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItemAdicionais", x => new { x.IdPedidoItem, x.IdAdicional });
                    table.ForeignKey(
                        name: "FK_PedidoItemAdicionais_Adicionais_IdAdicional",
                        column: x => x.IdAdicional,
                        principalTable: "Adicionais",
                        principalColumn: "IdAdicional",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoItemAdicionais_PedidoItens_IdPedidoItem",
                        column: x => x.IdPedidoItem,
                        principalTable: "PedidoItens",
                        principalColumn: "IdPedidoItem",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    IdPerfil = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NmPerfil = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    UsuarioPerfilIdUsuarioPerfil = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.IdPerfil);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPerfis",
                columns: table => new
                {
                    IdUsuarioPerfil = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdPerfil = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPerfis", x => x.IdUsuarioPerfil);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfis_Perfis_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfis",
                        principalColumn: "IdPerfil",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfis_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adicionais_IdLoja",
                table: "Adicionais",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_IdPedido",
                table: "Avaliacoes",
                column: "IdPedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bairros_IdLoja",
                table: "Bairros",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaFechamentos_IdLoja",
                table: "CaixaFechamentos",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaFechamentos_IdUsuario",
                table: "CaixaFechamentos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMovimentos_IdLoja",
                table: "CaixaMovimentos",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMovimentos_IdUsuario",
                table: "CaixaMovimentos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_IdLoja",
                table: "Categorias",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Cnpj",
                table: "Empresas",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_IdBairro",
                table: "Enderecos",
                column: "IdBairro");

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_IdCliente",
                table: "Enderecos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_KdsLogs_IdPedido",
                table: "KdsLogs",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_KdsLogs_IdUsuario",
                table: "KdsLogs",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_KdsPedidoItens_IdKdsPedido",
                table: "KdsPedidoItens",
                column: "IdKdsPedido");

            migrationBuilder.CreateIndex(
                name: "IX_KdsPedidoItens_IdProduto",
                table: "KdsPedidoItens",
                column: "IdProduto");

            migrationBuilder.CreateIndex(
                name: "IX_KdsPedidos_IdLoja",
                table: "KdsPedidos",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_KdsPedidos_IdPedido",
                table: "KdsPedidos",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Lojas_IdEmpresa",
                table: "Lojas",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemAdicionais_IdAdicional",
                table: "PedidoItemAdicionais",
                column: "IdAdicional");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_IdPedido",
                table: "PedidoItens",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_IdProduto",
                table: "PedidoItens",
                column: "IdProduto");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItens_IdVariacao",
                table: "PedidoItens",
                column: "IdVariacao");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdCliente",
                table: "Pedidos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdEndereco",
                table: "Pedidos",
                column: "IdEndereco");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdEntregador",
                table: "Pedidos",
                column: "IdEntregador");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdLoja",
                table: "Pedidos",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_UsuarioPerfilIdUsuarioPerfil",
                table: "Perfis",
                column: "UsuarioPerfilIdUsuarioPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IdCategoria",
                table: "Produtos",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IdLoja",
                table: "Produtos",
                column: "IdLoja");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoVariacoes_IdVariacao",
                table: "ProdutoVariacoes",
                column: "IdVariacao");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfis_IdPerfil",
                table: "UsuariosPerfis",
                column: "IdPerfil");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfis_IdUsuario",
                table: "UsuariosPerfis",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfis_UsuariosPerfis_UsuarioPerfilIdUsuarioPerfil",
                table: "Perfis",
                column: "UsuarioPerfilIdUsuarioPerfil",
                principalTable: "UsuariosPerfis",
                principalColumn: "IdUsuarioPerfil",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosPerfis_Usuarios_IdUsuario",
                table: "UsuariosPerfis");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfis_UsuariosPerfis_UsuarioPerfilIdUsuarioPerfil",
                table: "Perfis");

            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "CaixaFechamentos");

            migrationBuilder.DropTable(
                name: "CaixaMovimentos");

            migrationBuilder.DropTable(
                name: "KdsLogs");

            migrationBuilder.DropTable(
                name: "KdsPedidoItens");

            migrationBuilder.DropTable(
                name: "PedidoItemAdicionais");

            migrationBuilder.DropTable(
                name: "ProdutoVariacoes");

            migrationBuilder.DropTable(
                name: "KdsPedidos");

            migrationBuilder.DropTable(
                name: "Adicionais");

            migrationBuilder.DropTable(
                name: "PedidoItens");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Variacoes");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Bairros");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Lojas");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "UsuariosPerfis");

            migrationBuilder.DropTable(
                name: "Perfis");
        }
    }
}
