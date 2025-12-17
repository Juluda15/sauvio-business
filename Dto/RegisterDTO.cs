using System.ComponentModel.DataAnnotations;

namespace Sauvio.Business.Dto
{
    public class RegisterDTO
    {
        [Required]
        public string? Name { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
