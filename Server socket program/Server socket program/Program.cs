using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using EncryptLib;

namespace Server_socket_program
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 13356;                                       //My connection setup
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndpoint = new IPEndPoint(ip, port);

            TcpListener listener = new TcpListener(localEndpoint);
            listener.Start();

            Console.WriteLine("Awaiting Clients");
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client Connected");
            NetworkStream stream = client.GetStream();                 //Connected
            Encryption cryp = new Encryption(keychat(stream));
            string message = "";
            while (client.Connected)                                 //My loop for checking if it need to read a message or send one.
            {

                if (stream.DataAvailable)
                {
                    Console.WriteLine(cryp.AESdecrypted(ReadMessage(stream)));
                }
                else if (Console.KeyAvailable)                                      //Generate my message to send
                {
                    ConsoleKeyInfo keypressed = Console.ReadKey();
                    if (keypressed.Key == ConsoleKey.Enter && message.Length > 0)
                    {
                        SendMessage(stream, cryp.encrypt(message));
                        Console.WriteLine(message);
                        Console.WriteLine(cryp.encrypt(message));
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
                    }                                                               //End of generating my message to send
                }

            
                
            }

        }
        static void SendMessage(NetworkStream stream, string message)      //My send message to the stream
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
        static string ReadMessage(NetworkStream stream)                     //My read message from the stream
        {
            byte[] buffer = new byte[256];

            int numberOfBytesRead = stream.Read(buffer, 0, 256);
            return Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead);

        }
        static string keychat(NetworkStream stream)                     //Diffie Helman communication
        {
            double key;
            Keyexchange keyex = new Keyexchange();
            SendMessage(stream, keyex.p.ToString());
            keyex.CheckP(Double.Parse(ReadMessage(stream)));
            SendMessage(stream, keyex.publickey.ToString());
            keyex.generatepublickey(Double.Parse(ReadMessage(stream)));
            SendMessage(stream, keyex.Sendkey().ToString());
            key = keyex.generatekey(Double.Parse(ReadMessage(stream)));

            Console.WriteLine(key.ToString());

            return key.ToString();
        }
    }
}
