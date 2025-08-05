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

                Console.WriteLine($"🌐 Connecting to {_ip}:{_port}...");
                await client.ConnectAsync(_ip, _port);
                Console.WriteLine("✅ Connection established!");

                using NetworkStream stream = client.GetStream();

                // Frame HL7 message using MLLP
                string mllpMessage = "\x0B" + hl7Message + "\x1C\r";
                byte[] bytes = Encoding.UTF8.GetBytes(mllpMessage);

                Console.WriteLine("🧾 Sending HL7 Message:\n" + mllpMessage
                    .Replace("\x0B", "[VT]")
                    .Replace("\x1C", "[FS]")
                    .Replace("\r", "[CR]"));

                // Send message
                await stream.WriteAsync(bytes, 0, bytes.Length);
                await stream.FlushAsync();

                // Read ACK
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string ackRaw = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string ackVisible = ackRaw.Replace("\x0B", "[VT]").Replace("\x1C", "[FS]")
                                              .Replace("\r", "[CR]\n");

                    Console.WriteLine("📨 Received ACK:\n" + ackVisible);

                    // Extract MSA segment
                    string msaSegment = ackRaw.Split('\r').FirstOrDefault(s => s.StartsWith("MSA|")) ?? "";
                    if (msaSegment != null)
                    {
                        string[] fields = msaSegment.Split('|');
                        string ackCode = fields.Length > 1 ? fields[1] : "";

                        switch (ackCode)
                        {
                            case "AA":
                                Console.WriteLine("✅ Valid ACK received: Application Accept.");
                                return true;

                            case "AE":
                                Console.WriteLine("! Application Error.");
                                break;

                            case "AR":
                                Console.WriteLine("⚠️ Application Reject.");
                                break;

                            case "CA":
                                Console.WriteLine("✅ Commit Accept.");
                                break;

                            case "CE":
                                Console.WriteLine("❗ Commit Error.");
                                break;

                            case "CR":
                                Console.WriteLine("⚠️ Commit Reject.");
                                break;

                            default:
                                Console.WriteLine("❓ Unknown ACK code: " + ackCode);
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ No MSA segment found in ACK.");
                    }

                    return true; 
                }
                else
                {
                    Console.WriteLine("⚠️ No ACK received.");
                    return false;
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("❌ Could not connect to server: " + se.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ HL7 Send Error: " + ex.Message);
                return false;
            }
        }
    }
}
