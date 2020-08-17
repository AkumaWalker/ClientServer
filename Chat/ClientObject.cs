using System;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }

        private string UserName;
        private readonly TcpClient Client;
        private readonly ServerObject Server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            Client = tcpClient;
            Server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = Client.GetStream();
                // get username
                string message = GetMessage();
                UserName = message;

                message = UserName + " entered the chat";
                // send a message about entering the chat to all connected users
                Server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                // receive messages from the client in an endless loop
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", UserName, message);
                        Console.WriteLine(message);
                        Server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: left chat", UserName);
                        Console.WriteLine(message);
                        Server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Server.RemoveConnection(this.Id);
                Close();
            }
        }

        // read incoming message and convert to string
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();

            do
            {
                int bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // close connection
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (Client != null)
                Client.Close();
        }
    }
}
