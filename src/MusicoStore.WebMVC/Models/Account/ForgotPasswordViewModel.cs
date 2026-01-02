using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models.Account;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
