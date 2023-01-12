public class PatientData
{
    public string AHN { get; set; }
    public string Name { get; set; }
    public string DOB { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<AppointmentData> Appointments { get; set; }
}