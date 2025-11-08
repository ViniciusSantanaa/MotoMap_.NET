using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.Dtos
{
    public class CreateYardDto
    {
        [Required(ErrorMessage = "O nome do pátio é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome do pátio deve ter no máximo 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço do pátio é obrigatório.")]
        [MaxLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres.")]
        public string Address { get; set; } = string.Empty;
    }
}
