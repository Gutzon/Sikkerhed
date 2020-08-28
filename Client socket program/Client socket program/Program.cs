using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using EncryptLib;

namespace Client_socket_program
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            int port = 13356;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            client.Connect(endPoint);
            NetworkStream stream = client.GetStream();
            Encryption cryp = new Encryption(keychat(stream));
            string message = "";
            while (client.Connected)
            {
                if (stream.DataAvailable)
                {
                    Console.WriteLine(cryp.decrypt(ReadMessage(stream)));
                }
                else if (Console.KeyAvailable)
                {
                    
                    ConsoleKeyInfo keypressed = Console.ReadKey();
                    if (keypressed.Key == ConsoleKey.Enter && message.Length > 0)
                    {
                        SendMessage(stream, cryp.AESencrypted(message));
                        Console.WriteLine(message);
                        message = "";
                    }
                    else if (keypressed.Key == ConsoleKey.Backspace)
                    {
                        if (message.Length > 0)
                        {
                            message = message.Remove(message.Length - 1, 1);
                        }
                    }
                    else
                    {
                        message += keypressed.KeyChar.ToString();
                    }
                }

            }

            client.Close();

        }
        static void SendMessage(NetworkStream stream, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
        static string ReadMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[256];

            int numberOfBytesRead = stream.Read(buffer, 0, 256);
            return Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead);
            
        }
        static string keychat(NetworkStream stream)
        {
            double key;
            double tal;
            Keyexchange keyex = new Keyexchange();
            tal = Double.Parse(ReadMessage(stream));
            SendMessage(stream, keyex.p.ToString());
            keyex.CheckP(tal);
            tal = Double.Parse(ReadMessage(stream));
            SendMessage(stream, keyex.publickey.ToString());
            keyex.generatepublickey(tal);
            tal = Double.Parse(ReadMessage(stream));
            SendMessage(stream, keyex.Sendkey().ToString());
            key = keyex.generatekey(Double.Parse(tal.ToString()));


            Console.WriteLine(key.ToString());


            return key.ToString();
        }
    }
    
}
