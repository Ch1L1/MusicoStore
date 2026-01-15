using System.ComponentModel.DataAnnotations;
using MusicoStore.Domain.Constants;

namespace WebMVC.Models.Account;

public class RegisterViewModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    
    [Required]
    public bool Employee { get; set; }
    
    [Required]
    public string StreetName { get; set; }

    [Required]
    public string StreetNumber { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    [StringLength(maximumLength: 6, ErrorMessage = "PostalNumber must be max. 6 characters")]
    public string PostalNumber { get; set; }

    [Required]
    [StringLength(3, ErrorMessage = "Country code must be 3 letters (e.g. CZE, USA)")]
    public string CountryCode { get; set; }
}

