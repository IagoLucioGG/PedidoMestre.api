using Microsoft.EntityFrameworkCore;
using PedidoMestre.Models.Avaliacoes;
using PedidoMestre.Models.Caixa;
using PedidoMestre.Models.Clientes;
using PedidoMestre.Models.Empresas;
using PedidoMestre.Models.Geral;
using PedidoMestre.Models.Pedidos;
using PedidoMestre.Models.Produtos;
using PedidoMestre.Models.Usuarios;

namespace PedidoMestre.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfil> UsuariosPerfis { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Loja> Lojas { get; set; }
        public DbSet<Bairro> Bairros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Variacao> Variacoes { get; set; }
        public DbSet<ProdutoVariacao> ProdutoVariacoes { get; set; }
        public DbSet<Adicional> Adicionais { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<PedidoItemAdicional> PedidoItemAdicionais { get; set; }
        public DbSet<KdsPedido> KdsPedidos { get; set; }
        public DbSet<KdsPedidoItem> KdsPedidoItens { get; set; }
        public DbSet<KdsLog> KdsLogs { get; set; }
        public DbSet<CaixaMovimento> CaixaMovimentos { get; set; }
        public DbSet<CaixaFechamento> CaixaFechamentos { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações finas de relacionamento
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UsuarioPerfil)
                .WithOne(up => up.Usuario)
                .HasForeignKey<UsuarioPerfil>(up => up.IdUsuario);

            modelBuilder.Entity<UsuarioPerfil>()
                .HasOne(up => up.Perfil)
                .WithMany()
                .HasForeignKey(up => up.IdPerfil);

            // Configuração do relacionamento Empresa -> Lojas (1 para muitos)
            modelBuilder.Entity<Loja>()
                .HasOne(l => l.Empresa)
                .WithMany(e => e.Lojas)
                .HasForeignKey(l => l.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de índice único para CNPJ da Empresa
            modelBuilder.Entity<Empresa>()
                .HasIndex(e => e.Cnpj)
                .IsUnique();

            // Configuração do tipo JSONB para ConfigDelivery da Loja
            modelBuilder.Entity<Loja>()
                .Property(l => l.ConfigDelivery)
                .HasColumnType("jsonb");

            // Configuração do relacionamento Loja -> Categorias (1 para muitos)
            modelBuilder.Entity<Categoria>()
                .HasOne(c => c.Loja)
                .WithMany()
                .HasForeignKey(c => c.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Categoria -> Produtos (1 para muitos)
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> Produtos (1 para muitos)
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Loja)
                .WithMany()
                .HasForeignKey(p => p.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento N:N entre Produto e Variacao
            modelBuilder.Entity<ProdutoVariacao>()
                .HasKey(pv => new { pv.IdProduto, pv.IdVariacao });

            modelBuilder.Entity<ProdutoVariacao>()
                .HasOne(pv => pv.Produto)
                .WithMany(p => p.ProdutoVariacoes)
                .HasForeignKey(pv => pv.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoVariacao>()
                .HasOne(pv => pv.Variacao)
                .WithMany(v => v.ProdutoVariacoes)
                .HasForeignKey(pv => pv.IdVariacao)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> Adicionais (1 para muitos)
            modelBuilder.Entity<Adicional>()
                .HasOne(a => a.Loja)
                .WithMany()
                .HasForeignKey(a => a.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> Bairros (1 para muitos)
            modelBuilder.Entity<Bairro>()
                .HasOne(b => b.Loja)
                .WithMany(l => l.Bairros)
                .HasForeignKey(b => b.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Cliente -> Enderecos (1 para muitos)
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Cliente)
                .WithMany(c => c.Enderecos)
                .HasForeignKey(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Bairro -> Enderecos (1 para muitos)
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Bairro)
                .WithMany(b => b.Enderecos)
                .HasForeignKey(e => e.IdBairro)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de índice único para Email do Cliente (apenas quando não for nulo)
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("\"Email\" IS NOT NULL");

            // Configuração do relacionamento Loja -> Pedidos (1 para muitos)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Loja)
                .WithMany()
                .HasForeignKey(p => p.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Cliente -> Pedidos (1 para muitos)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Endereco -> Pedidos (1 para muitos)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Endereco)
                .WithMany()
                .HasForeignKey(p => p.IdEndereco)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Usuario (Entregador) -> Pedidos (1 para muitos)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Entregador)
                .WithMany()
                .HasForeignKey(p => p.IdEntregador)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> Usuarios (1 para muitos)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Loja)
                .WithMany()
                .HasForeignKey(u => u.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Pedido -> PedidoItens (1 para muitos)
            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(pi => pi.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Produto -> PedidoItens (1 para muitos)
            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Produto)
                .WithMany()
                .HasForeignKey(pi => pi.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Variacao -> PedidoItens (1 para muitos, opcional)
            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Variacao)
                .WithMany()
                .HasForeignKey(pi => pi.IdVariacao)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento N:N entre PedidoItem e Adicional
            modelBuilder.Entity<PedidoItemAdicional>()
                .HasKey(pia => new { pia.IdPedidoItem, pia.IdAdicional });

            modelBuilder.Entity<PedidoItemAdicional>()
                .HasOne(pia => pia.PedidoItem)
                .WithMany(pi => pi.Adicionais)
                .HasForeignKey(pia => pia.IdPedidoItem)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PedidoItemAdicional>()
                .HasOne(pia => pia.Adicional)
                .WithMany()
                .HasForeignKey(pia => pia.IdAdicional)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Pedido -> KdsPedidos (1 para 1)
            modelBuilder.Entity<KdsPedido>()
                .HasOne(kp => kp.Pedido)
                .WithMany()
                .HasForeignKey(kp => kp.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> KdsPedidos (1 para muitos)
            modelBuilder.Entity<KdsPedido>()
                .HasOne(kp => kp.Loja)
                .WithMany()
                .HasForeignKey(kp => kp.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento KdsPedido -> KdsPedidoItens (1 para muitos)
            modelBuilder.Entity<KdsPedidoItem>()
                .HasOne(kpi => kpi.KdsPedido)
                .WithMany(kp => kp.Itens)
                .HasForeignKey(kpi => kpi.IdKdsPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Produto -> KdsPedidoItens (1 para muitos)
            modelBuilder.Entity<KdsPedidoItem>()
                .HasOne(kpi => kpi.Produto)
                .WithMany()
                .HasForeignKey(kpi => kpi.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Pedido -> KdsLogs (1 para muitos)
            modelBuilder.Entity<KdsLog>()
                .HasOne(kl => kl.Pedido)
                .WithMany()
                .HasForeignKey(kl => kl.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Usuario -> KdsLogs (1 para muitos, opcional)
            modelBuilder.Entity<KdsLog>()
                .HasOne(kl => kl.Usuario)
                .WithMany()
                .HasForeignKey(kl => kl.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> CaixaMovimentos (1 para muitos)
            modelBuilder.Entity<CaixaMovimento>()
                .HasOne(cm => cm.Loja)
                .WithMany()
                .HasForeignKey(cm => cm.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Usuario -> CaixaMovimentos (1 para muitos)
            modelBuilder.Entity<CaixaMovimento>()
                .HasOne(cm => cm.Usuario)
                .WithMany()
                .HasForeignKey(cm => cm.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Loja -> CaixaFechamentos (1 para muitos)
            modelBuilder.Entity<CaixaFechamento>()
                .HasOne(cf => cf.Loja)
                .WithMany()
                .HasForeignKey(cf => cf.IdLoja)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Usuario -> CaixaFechamentos (1 para muitos)
            modelBuilder.Entity<CaixaFechamento>()
                .HasOne(cf => cf.Usuario)
                .WithMany()
                .HasForeignKey(cf => cf.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento Pedido -> Avaliacoes (1 para 1)
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Pedido)
                .WithMany()
                .HasForeignKey(a => a.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de índice único para Pedido (uma avaliação por pedido)
            modelBuilder.Entity<Avaliacao>()
                .HasIndex(a => a.IdPedido)
                .IsUnique();
        }

    }
}