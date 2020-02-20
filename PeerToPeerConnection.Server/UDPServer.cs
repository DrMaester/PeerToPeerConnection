using PeerToPeerConnection.Server.Model;
using PeerToPeerConnection.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PeerToPeerConnection.Server
{
    public class UDPServer
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint _localEndPoint;
        private readonly object _clientsLock;
        private List<ClientData> _clients;
        private const int _packetBuffersize = 8 * 1024;

        public UDPServer(int port)
        {
            _clientsLock = new object();
            _clients = new List<ClientData>();
            _localEndPoint = new IPEndPoint(IPAddress.Any, port);
            _socket.Bind(_localEndPoint);
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[_packetBuffersize];
                    var readed = _socket.ReceiveFrom(buffer, ref _localEndPoint);
                    buffer = buffer.Take(readed).ToArray();

                    if (!_clients.Any(client => client.IPEndPoint.Address.ToString() == ((IPEndPoint)_localEndPoint).Address.ToString() &&
                        client.IPEndPoint.Port == ((IPEndPoint)_localEndPoint).Port))
                    {
                        lock (_clientsLock)
                        {
                            _clients.Add(new ClientData(((IPEndPoint)_localEndPoint)));
                            Console.WriteLine($"Client connected {_clients.Last().IPEndPoint}");
                        }
                    }

                    var paket = new Paket(buffer);
                    DataHandler(paket, (IPEndPoint)_localEndPoint);
                }
            });
            Console.WriteLine($"server running on {_localEndPoint}");
        }

        private void DataHandler(Paket receivedPaket, IPEndPoint sender)
        {
            switch (receivedPaket.PaketType)
            {
                case PaketType.RequestExternalAddress:
                    var data = Encoding.UTF8.GetBytes(sender.ToString());
                    var paket = new Paket(PaketType.AnswerExternalAddress, data);
                    _socket.SendTo(paket.ToBytes(), sender);
                    break;
                default:
                    break;
            }
        }

        private string GetIdFromEndPoint(IPEndPoint iPEndPoint)
        {
            lock (_clientsLock)
            {
                return _clients.FirstOrDefault(client => client.IPEndPoint.Address.ToString() == iPEndPoint.Address.ToString() &&
                        client.IPEndPoint.Port == iPEndPoint.Port).Id.ToString();
            }
        }
    }
}
