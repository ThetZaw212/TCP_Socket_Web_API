﻿using System.Net.Sockets;
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

        public async Task<bool> SendMessageAsync(string hl7Message, int maxRetries = 3)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                attempt++;
                try
                {
                    using TcpClient client = new TcpClient();

                    Console.WriteLine($"🌐 Connecting to {_ip}:{_port}... (Attempt {attempt})");
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

                        if (ackRaw.Contains("MSA|AA"))
                        {
                            Console.WriteLine("✅ Valid ACK received");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("⚠️ ACK received but not AA. Retrying...");
                            await Task.Delay(1000); // Optional delay between retries
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠️ No ACK received. Retrying...");
                        await Task.Delay(1000);
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

            Console.WriteLine("❌ Failed after max retries.");
            return false;
        }
    }
}