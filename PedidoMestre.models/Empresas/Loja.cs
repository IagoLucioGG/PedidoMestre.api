using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidoMestre.Models.Empresas
{
    public class Loja
    {
        [Key]
        public int IdLoja { get; set; }

        public int IdEmpresa { get; set; }

        [Required]
        [MaxLength(255)]
        public string Endereco { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public string? ConfigDelivery { get; set; }

        // Coordenadas para cálculo de distância
        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        // Raio de entrega em quilômetros
        [Column(TypeName = "decimal(10,2)")]
        public decimal? RaioEntrega { get; set; }

        // Horários de funcionamento (pode ser JSON ou campos separados)
        [MaxLength(50)]
        public string? HorarioAbertura { get; set; }

        [MaxLength(50)]
        public string? HorarioFechamento { get; set; }

        // Relacionamento: Uma loja pertence a uma empresa
        public Empresa? Empresa { get; set; }

        // Relacionamento: Uma loja pode ter muitos bairros
        public ICollection<Geral.Bairro> Bairros { get; set; } = new List<Geral.Bairro>();
    }
}

