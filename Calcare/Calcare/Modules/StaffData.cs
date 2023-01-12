
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class StaffData
{
    
    
    public string Userid { get; set; }
    [Required]
    
    [StringLength(50)]
    public string Name { get; set; }
    [Required]
    [MinLength(8, ErrorMessage ="Password must be of atleast 8 characters")]
    public string Password { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(50)]
    public string Clinic { get; set; }
    public string user { get; set; }
}
