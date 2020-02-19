using PeerToPeerConnection.Shared.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PeerToPeerConnection.Client
{
    public class UdpService : IDisposable
    {
        private int _port;
        private IPAddress _serverIp;
        private IPEndPoint _serverIPEndpoint;
        private UdpClient _client;
        private const int _paketSize = 8 * 1024;

        public UdpService(IPAddress iPAddress, int port)
        {
            _serverIp = iPAddress;
            _port = port;
            _serverIPEndpoint = new IPEndPoint(_serverIp, _port);
            _client = new UdpClient();
        }

        public void SendPaketToPeer()
        {
            Console.Write("Enter iPAddress: ");
            var ipaddress = Console.ReadLine();
            Console.Write("Enter Port: ");
            var port = Console.ReadLine();
            Console.WriteLine("Sending..");

            var paket = new Paket(PaketType.Greeting, null);
            var paketBuffer = paket.ToBytes();
            var remotePeerEndpoint = new IPEndPoint(IPAddress.Parse(ipaddress), int.Parse(port));

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(remotePeerEndpoint);
            socket.SendTo(paketBuffer, remotePeerEndpoint);

        }

        public void AskForExternalAddress()
        {
            var paket = new Paket(PaketType.RequestExternalAddress, null);
            var paketBuffer = paket.ToBytes();
            _client.Send(paketBuffer, paketBuffer.Length);
        }

        public void Connect()
        {
            Task.Run(() => Receive());
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private void Receive()
        {
            try
            {
                _client.Connect(_serverIPEndpoint);
                var connectPaket = new Paket(PaketType.Connect, null);
                var connectPaketBuffer = connectPaket.ToBytes();
                _client.Send(connectPaketBuffer, connectPaketBuffer.Length);
                while (true)
                {
                    var paketBuffer = _client.Receive(ref _serverIPEndpoint);
                    var paket = new Paket(paketBuffer);
                    DataHandler(paket);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("\n$$$ -> " + ex.Message + "\n");
            }
        }

        private void DataHandler(Paket paket)
        {
            switch (paket.PaketType)
            {
                case PaketType.AnswerExternalAddress:
                    var ipEndPoint = IPEndPoint.Parse(Encoding.UTF8.GetString(paket.Data));
                    Console.WriteLine($"\n$$$ -> External address: {ipEndPoint}\n");
                    break;
                case PaketType.Greeting:
                    Console.WriteLine("Greetings from another peer");
                    break;
                default:
                    break;
            }
        }
    }
}
