using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        private const string Host = "127.0.0.1";
        private const int Port = 8888;

        private static string UserName;
        private static TcpClient Client;
        private static NetworkStream Stream;

        static void Main(string[] args)
        {
            Console.Write("Input name: ");
            UserName = Console.ReadLine();
            Client = new TcpClient();

            try
            {
                Client.Connect(Host, Port);
                Stream = Client.GetStream();

                string message = UserName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                Stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine("Welcome, {0}", UserName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage()
        {
            Console.WriteLine("Input message: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                Stream.Write(data, 0, data.Length);
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = Stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (Stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Connect error!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (Stream != null)
                Stream.Close();
            if (Client != null)
                Client.Close();

            Environment.Exit(0);
        }
    }
}
