using System.ComponentModel.DataAnnotations;

public class PatientDataForm
{
    [Required(ErrorMessage = "Alberta Health Care Number is required, and must follow the format: XXXXX-XXXX")]
    [RegularExpression(@"^\d{5}-\d{4}$", ErrorMessage = "Alberta Health Care Number is required, and must follow the format: XXXXX-XXXX")]
    public string AHN { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public string DOB { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [Phone]
    public string PhoneNumber { get; set; }
}