using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
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
        public string name;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            count += sizeof(ushort);

            //this.playerId = BitConverter.ToInt64(s.Array, s.Offset + count);
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), (ushort)4);
            //위의 코드처럼 size값을 잘못 보냈을 떄, 이를 감지할 수 있게 코드 수정
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));

        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            bool success = true;
            ushort count = 0;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            //Span<byte>에 정보를 입력한다

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);



            //string
            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            //success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            //count += sizeof(ushort);
            //Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            //count += nameLen;

            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;
            //packetsize는 count 계산이 끝난 맨마지막에, 첫 offset에 넣어준다
            success &= BitConverter.TryWriteBytes(s, count);

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


    class ClientSession : PacketSession
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
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            
            //ushort playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            //Console.WriteLine($"PlayerInfoReq1: {playerId}");

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        //long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                        //count += 8;

                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");
                    }
                    break;
            } 

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
}
