namespace TCP_Socket_Web_API.Models
{
    public class Hl7OrderRequest
    {
        public string PatientId { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string DOB { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string DoctorId { get; set; } = null!;
        public string DoctorName { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public string SampleType { get; set; } = null!;
        public string OrderTime { get; set; } = null!;
        public string CollectionTime { get; set; } = null!;
        public List<string> TestCodes { get; set; } = null!;
    }
}
