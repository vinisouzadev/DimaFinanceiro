using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Account
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }
}
