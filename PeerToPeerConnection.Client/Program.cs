using System;
using System.Net;

namespace PeerToPeerConnection.Client
{
    class Program
    {
        private static IPAddress _ipAddress = IPAddress.Parse("127.0.0.1");
        private static int _port = 27005;
        private static UdpService _udpService;

        static void Main(string[] args)
        {
            ConnectToUDPServer();
            char pressedKey = ' ';
            do
            {
                ShowMenu();
                pressedKey = Console.ReadKey(false).KeyChar;
                switch (pressedKey)
                {
                    case '0':
                        _udpService.AskForExternalAddress();
                        break;
                    case '1':
                        _udpService.SendPaketToPeer();
                        break;
                    default:
                        break;
                }

                ClearConsoleKeyBuffer();
                Console.WriteLine("\n\n");
            } while (pressedKey != 'q');

            _udpService.Dispose();
        }

        private static void ClearConsoleKeyBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey();
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("~~~MENUE~~~");
            Console.WriteLine("0 - Ask for external endpoint address");
            Console.WriteLine("1 - Send a paket to another peer");
            Console.WriteLine("q - quit application");
        }

        private static void ConnectToUDPServer()
        {
            _udpService = new UdpService(_ipAddress, _port);
            _udpService.Connect();
        }

    }
}
