using System.ComponentModel.DataAnnotations;

namespace Online_Store.Models
{
    public class ViewUser
    {

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

    }
}
