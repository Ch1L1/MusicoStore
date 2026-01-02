using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models.Address;

public class AddressFormViewModel
{
    public int Id { get; set; }

    // [Required(ErrorMessage = "Ulice je povinná")]
    // [Display(Name = "Ulice")]
    public string StreetName { get; set; } = string.Empty;

    // [Required(ErrorMessage = "Číslo popisné je povinné")]
    // [Display(Name = "Č.P.")]
    public string StreetNumber { get; set; } = string.Empty;

    // [Required(ErrorMessage = "Město je povinné")]
    // [Display(Name = "Město")]
    public string City { get; set; } = string.Empty;

    // [Required(ErrorMessage = "PSČ je povinné")]
    // [Display(Name = "PSČ")]
    public string PostalNumber { get; set; } = string.Empty;

    // [Required(ErrorMessage = "Kód země je povinný")]
    // [Display(Name = "Země (kód)")]
    [StringLength(3, ErrorMessage = "Max 3 characters (e.g. CZE)")]
    public string CountryCode { get; set; } = string.Empty;
}
