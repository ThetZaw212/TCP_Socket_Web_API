using System.Net.Sockets;
using System.Text;

namespace TCP_Socket_Web_API.Services
{
    public class Hl7TcpSenderService
    {
        private readonly string _ip;
        private readonly int _port;

        public Hl7TcpSenderService(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public async Task<bool> SendMessageAsync(string hl7Message)
        {
            try
            {
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(_ip, _port);

                using NetworkStream stream = client.GetStream();

                string mllpMessage = $"\x0B{hl7Message}\x1C\r";
                byte[] bytes = Encoding.UTF8.GetBytes(mllpMessage);

                await stream.WriteAsync(bytes, 0, bytes.Length);

                // Optionally read ACK
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("ACK from LIS: " + response);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HL7 Send Error: " + ex.Message);
                return false;
            }
        }
    }
}
