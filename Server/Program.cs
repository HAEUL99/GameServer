using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Packet 
    {
        public ushort size;
        public ushort packetId;
    }

    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            //문자열 보내기
            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");

            /*
            Packet packet = new Packet() { size = 4, packetId = 7 };

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            Send(sendBuff);
            */
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId: {id}, Size: {size}");

        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        

        public override void OnSend(int numOfBytes)
        {

            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }

    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            //DNS(Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening ..");

            while (true)
            {

            }



        }
    }
}
