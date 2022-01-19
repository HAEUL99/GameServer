# GameServer
[C#과 유니티로 만드는 MMORPG 게임 개발 시리즈] Part4: 게임 서버

## DummyClient
- Program.cs
  - void Main(string[] args): EndPoint 설정후 Connector.Connect 호출
- ServerSession.cs
## Server
- Program.cs
- ClientSession.cs
## ServerCore
- Conncetor.cs
  - void Connect(IPEndPoint endPoint, Func<Session> sessionFactory): socket 생성후, completed 이벤트 등록
  - void RegisterConnect(SocketAsyncEventArgs args): socket 연결 등록 (socket.ConncetAsync(args)) 
  - void OnConnectCompleted(object sender, SocketAsyncEventArgs args): socket 연결 이벤트 발생하면, sessionFactory.Invoke하여 Serversession 생성. Serversession Start, OnConnected함수 호출.
  
- Listener.cs
- RecvBuffer.cs
- SendBuffer.cs
- Session.cs
   - Session(abstract)
      - Start(Socket socket)
      - Send(ArraySegment<byte> sendBuff)
      - Disconnect()
      - RegisterSend()
      - OnSendCompleted(object sender, SocketAsyncEventArgs args) 
      - RegisterRecv() 
      - OnRecvCompleted(object sender, SocketAsyncEventArgs args) 
   
   - PacketSession: Session(abstract)
      - int OnRecv(ArraySegment<byte> buffer) 
      - void OnRecvPacket(ArraySegment<byte> buffer);

  
  ## 흐름 순서
  Program(DummyClient) -> Connector -> Serversession.Start -> Serversession.OnConnected
