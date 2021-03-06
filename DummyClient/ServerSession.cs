using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{

	class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            /*
            C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name = "ABCD"};
			var skill = new C_PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f };
			skill.attributes.Add(new C_PlayerInfoReq.Skill.Attribute() { att = 77 });
			packet.skills.Add(skill);

            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 201, level = 2, duration = 3.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 301, level = 3, duration = 3.0f });
            packet.skills.Add(new C_PlayerInfoReq.Skill() { id = 401, level = 4, duration = 3.0f });
            ArraySegment<byte> s = packet.Write();
            if (s != null)
                Send(s);
            */
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
