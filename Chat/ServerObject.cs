using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    public class ServerObject
    {
        private static TcpListener TcpListener;
        private readonly List<ClientObject> Clients = new List<ClientObject>();

        public void AddConnection(ClientObject clientObject)
        {
            Clients.Add(clientObject);
        }

        public void RemoveConnection(string id)
        {
            ClientObject client = Clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
                Clients.Remove(client);
        }

        // listening for incoming connections
        public void Listen()
        {
            try
            {
                TcpListener = new TcpListener(IPAddress.Any, 8888);
                TcpListener.Start();

                Console.WriteLine("The server is running. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = TcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // broadcast message to connected clients
        public void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < Clients.Count; i++)
            {
                if (Clients[i].Id != id)
                {
                    Clients[i].Stream.Write(data, 0, data.Length); // data transfer
                }
            }
        }

        // disconnecting all clients
        public void Disconnect()
        {
            TcpListener.Stop();

            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Close();
            }

            Environment.Exit(0);
        }
    }
}
