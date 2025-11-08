using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.Dtos
{
    public class CreateReaderDto
    {
        [Required(ErrorMessage = "O número de série é obrigatório.")]
        [StringLength(100, ErrorMessage = "O número de série deve ter no máximo 100 caracteres.")]
        public string SerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição da localização é obrigatória.")]
        [StringLength(200, ErrorMessage = "A descrição da localização deve ter no máximo 200 caracteres.")]
        public string LocationDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do pátio é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do pátio deve ser maior que zero.")]
        public int YardId { get; set; }
    }
}

