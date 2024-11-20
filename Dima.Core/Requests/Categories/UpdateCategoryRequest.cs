using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Categories
{
    public class UpdateCategoryRequest : Request
    {
        [Required(ErrorMessage = "O Id é obrigatório")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(80, ErrorMessage = "Este campo deve conter no máximo {1} caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string? Description { get; set; }
    }
}
