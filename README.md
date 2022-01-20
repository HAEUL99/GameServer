# GameServer
[C#과 유니티로 만드는 MMORPG 게임 개발 시리즈] Part4: 게임 서버

## DummyClient
- Program.cs
  - void Main(string[] args): EndPoint 설정후 Connector.Connect 호출
- ServerSession.cs

## Server
- Program.cs
- ClientSession.cs

## ServerCore(라이브러리로 사용)
- Conncetor.cs
  - void Connect(IPEndPoint endPoint, Func<Session> sessionFactory): socket 생성후, completed 이벤트 등록함.
  - void RegisterConnect(SocketAsyncEventArgs args): socket 연결 등록함. (socket.ConncetAsync(args)) 
  - void OnConnectCompleted(object sender, SocketAsyncEventArgs args): socket 연결 이벤트 발생하면, sessionFactory.Invoke하여 Serversession 생성함. Serversession Start, OnConnected함수 호출함.
  
- Listener.cs
- RecvBuffer.cs
- SendBuffer.cs
  
- Session.cs
   - Session(abstract)
      - Start(Socket socket): receive, send용 SocketAsyncEventArgs에 Completed 이벤트를 연결함.
      - Send(ArraySegment<byte> sendBuff): Send 호출시, sendQueue에 sendBuff를 입력하고, 현재 보내고 있는 작업중이 없으면(_pendinglist.Count == 0) RegiseterSend 함수 호출함.
      - Disconnect(): Conncetor.Connect에서 생성하여 사용하던 socket을 해당 endPoint로 부터 disconnect함.
      - RegisterSend(): sendBuff에서 값을 꺼내어 _pendinglist에 추가하고, 이를 send용 SocketAsyncEventArgs에 bufferlist로 정의함. 연걸된 소켓에 데이터를 보냄. (socket.SendAsync(_sendArgs))
      - OnSendCompleted(object sender, SocketAsyncEventArgs args) : 소켓이 제대로 보내졌으면, bufferlist와 _pendinglist를 null처리함. 그리고 Onsend함수 호출. sendQueue에 값이 있으면(RegisterSend, OnSendCompleted 함수를 처리하는 동안, send함수가 발동하여 큐에 쌓이는 경우)  RegisterSend함수를 호출함.
      - RegisterRecv(): 
      - OnRecvCompleted(object sender, SocketAsyncEventArgs args) 
   
   - PacketSession: Session(abstract)
      - int OnRecv(ArraySegment<byte> buffer) 
      - void OnRecvPacket(ArraySegment<byte> buffer);

  
  ## 흐름 순서
  Program(DummyClient) -> Connector -> Serversession.Start -> Serversession.OnConnected
