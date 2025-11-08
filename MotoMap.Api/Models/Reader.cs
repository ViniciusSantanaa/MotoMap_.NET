using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoMap.Api.Models
{

    public class Reader
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O número de série é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O número de série deve ter no máximo 100 caracteres.")]
        public string SerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição da localização é obrigatória.")]
        [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        public string LocationDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do pátio é obrigatório.")]
        [ForeignKey(nameof(Yard))]
        public int YardId { get; set; }

        public Yard Yard { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
