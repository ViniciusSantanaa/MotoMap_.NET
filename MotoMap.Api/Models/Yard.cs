using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoMap.Api.Models
{
    public class Yard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do pátio é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do pátio deve ter no máximo 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço do pátio é obrigatório.")]
        [MaxLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres.")]
        public string Address { get; set; } = string.Empty;

        public ICollection<Reader> Readers { get; set; } = new List<Reader>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
