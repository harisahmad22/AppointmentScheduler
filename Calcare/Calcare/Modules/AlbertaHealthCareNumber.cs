using System.ComponentModel.DataAnnotations;

public class AlbertaHealthCareNumber
{
    [Required(ErrorMessage = "Alberta Health Care Number is required, and must follow the format: XXXXX-XXXX")]
    [RegularExpression(@"^\d{5}-\d{4}$", ErrorMessage = "Alberta Health Care Number is required, and must follow the format: XXXXX-XXXX")]
    public string AHN { get; set; }
}