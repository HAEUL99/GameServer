﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
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

                        foreach (PlayerInfoReq.Skill skill in p.skills) 
                        {
                            Console.WriteLine($"Skill {skill.id} {skill.level} {skill.duration}");
                        }
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
