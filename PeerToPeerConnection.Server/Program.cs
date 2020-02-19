using System;

namespace PeerToPeerConnection.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPServer server = new UDPServer(27005);
            server.Start();
            Console.ReadKey();
        }
    }
}
