using TCP_Socket_Web_API.Models;

namespace TCP_Socket_Web_API.Services
{
    public class Hl7TemplateBuilder
    {
        public static string BuildFromOrder(Hl7OrderRequest request)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string msgId = Guid.NewGuid().ToString("N");

            string msh = $"MSH|^~\\&|Ber|VH|LAB|VH|{timestamp}||OML^O21^OML_O21|{msgId}|P|2.5||||AL|||||";
            string pid = $"PID|1||{request.PatientId}||^{request.PatientName}||{request.DOB}|{request.Gender}|||{request.Address}|MMR||||||||||||||||||N";
            string pv1 = $"PV1|1|O|OUTP||||{request.DoctorId}^{request.DoctorName}||||||||||||O|||||||||||||||||||||||||{request.OrderTime}|";
            string orc = $"ORC||{request.OrderNumber}|{request.DoctorId}^{request.DoctorName}|BC|NW||{request.SampleType}||{request.OrderTime}|{request.CollectionTime}";

            var obrs = new List<string>();
            for (int i = 0; i < request.TestCodes.Count; i++)
            {
                string testCode = request.TestCodes[i];
                obrs.Add($"OBR|{i + 1}|||{testCode}||{request.OrderTime}|{request.OrderTime}||||A|||{request.OrderTime}");
            }

            return string.Join("\r", new[] { msh, pid, pv1, orc }.Concat(obrs));
        }
    }
}
