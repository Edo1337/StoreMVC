using System.ComponentModel.DataAnnotations;

namespace StoreMVC.Models
{
    public class Registr
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte Age { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordRepeat { get; set; }
    }
}
