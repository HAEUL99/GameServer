using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    public abstract class Packet
    {
        //패킷헤더
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet 
    {
        //패킷의 고유정보
        public long playerId;

        public PlayerInfoReq() 
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;

            count += 2;
            count += 2;

            //this.playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), (ushort)4);
            //위의 코드처럼 size값을 잘못 보냈을 떄, 이를 감지할 수 있게 코드 수정
            this.playerId = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);
            bool success = true;
            ushort count = 0;

            //Span<byte>에 정보를 입력한다

            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;
            //packetsize는 count 계산이 끝난 맨마지막에, 첫 offset에 넣어준다
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), count);

            if (success == false)
                return null;

            


            return SendBufferHelper.Close(count);
        }
    }

  

    public enum PacketID 
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }


    class ServerSession : Session
    {

        public override void OnConnected(EndPoint endPoint)
        {

            Console.WriteLine($"OnConnected: {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 };


            ArraySegment<byte> s = packet.Write();
            if (s != null)
                Send(s);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {

            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
