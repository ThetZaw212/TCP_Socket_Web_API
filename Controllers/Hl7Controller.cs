using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCP_Socket_Web_API.Models;
using TCP_Socket_Web_API.Services;

namespace TCP_Socket_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Hl7Controller : ControllerBase
    {
        [HttpPost("send")]
        [EndpointSummary("Send Message To LIS.")]
        public async Task<IActionResult> Send([FromBody] Hl7OrderRequest request)
        {
            string hl7Message = Hl7TemplateBuilder.BuildFromOrder(request);

            var sender = new Hl7TcpSenderService("192.168.100.253", 20860); 
            bool sent = await sender.SendMessageAsync(hl7Message);

            return sent ? Ok("HL7 sent successfully.") : StatusCode(500, "Failed to send HL7.");
        }

        [HttpPost("cancel")]
        [EndpointSummary("Send Cancel Message To LIS.")]
        public async Task<IActionResult> Cancel([FromBody] Hl7OrderRequest request)
        {
            // Build cancel HL7 message
            string hl7Message = Hl7TemplateBuilder.BuildCancelOrder(request);

            var sender = new Hl7TcpSenderService("192.168.100.253", 20860);
            bool sent = await sender.SendMessageAsync(hl7Message);

            return sent ? Ok("HL7 cancel sent successfully.") : StatusCode(500, "Failed to send HL7 cancel.");
        }

    }
}
