using System;
using System.Threading;

using Chat;

namespace Server
{
    class Program
    {
        private static ServerObject Server;
        private static Thread ListenThread;

        static void Main(string[] args)
        {
            try
            {
                Server = new ServerObject();
                ListenThread = new Thread(new ThreadStart(Server.Listen));
                ListenThread.Start();
            }
            catch (Exception ex)
            {
                Server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
