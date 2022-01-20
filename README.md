# GameServer
[C#과 유니티로 만드는 MMORPG 게임 개발 시리즈] Part4: 게임 서버

## DummyClient
- Program.cs
  - void Main(string[] args): EndPoint 설정후 Connector의 Connect 호출
- ServerSession.cs

## Server
- Program.cs
  - void Main(string[] args): EndPoint 설정후 Listener의 Init 호출
- ClientSession.cs

## ServerCore(라이브러리로 사용)
- Conncetor.cs
  - void Connect(IPEndPoint endPoint, Func<Session> sessionFactory): socket 생성후, completed 이벤트 등록함. RegisterConnect호출.
  - void RegisterConnect(SocketAsyncEventArgs args): socket 연결 등록함. (socket.ConncetAsync(args)) 
  - void OnConnectCompleted(object sender, SocketAsyncEventArgs args): socket 연결 이벤트 발생하면, sessionFactory.Invoke하여 ServerSession 생성함. ServerSession의 Start, OnConnected함수 호출함.
  
- Listener.cs
  - Init(IPEndPoint endPoint, Func<Session> sessionFactory): socket 생성후, Completed 이벤트 등록함. socket.Bind와 Listen 설정. RegisterAccept 호출
  - RegisterAccept(SocketAsyncEventArgs args): socket의 Accept를 등록함. (socket.AcceptAsync(args))
  - OnAcceptCompleted(object sender, SocketAsyncEventArgs args): socket accept 이벤트 발생하면, sessionFactory.Invoke하여 ClientSession 생성함. ClientSession의 Start, OnConncetd함수를 호출함.
  
- RecvBuffer.cs
  - RecvBuffer(int bufferSize)(생성자): bufferSize만큼의 공간이 있으며, 0부터 시작하는 byte[bufferSize]를 생성함.
  - DataSize (프로퍼티): 유효한 버퍼의 크기를 반환함(writePos - readPos)
  - FreeSize (프로퍼티): 남은 버퍼의 크기를 반환함(전체크기 - writePos)
  - ReadSegment (프로퍼티): 어디부터 어디까지 읽으면 되는지를 알려주는 ArraySegment를 반환함.
  - WriteSegment (프로퍼티): 어디부터 쓰면되는지를 알려주는 ArraySegment를 반환함.
  - Clean(): 버퍼앞에 공간이 있으면, readPos와 writePos의 위치를 앞으로 땡겨옴.
  - OnRead(int numOfBytes): 성공적으로 read하면, readPos 위치를 바꿔줌(numOfBytes > DataSize인 경우 실패)
  - OnWrite(int numOfBytes): 성공적으로 write하면, writePos 위치를 바꿔줌(numOfBytes > FreeSize인 경우 실패)

  - SendBuffer.cs
  - SendBufferHelper
    - ChunckSize (프로퍼티): get, set
    - Open(int reserveSize)(static): ThreadLocal<SendBuffer>가 이전에 생성된 적이 없거나, ThreadLocal<SendBuffer>의 남은 사이즈(FreeSize)가 reserveSize보다 작으면 SendBuffer를 새로 생성함. 그리고 SendBuffer의 Open함수를 호출하여 이값을 반환함.
    - Close(int usedSize)(static): SendBuffer의 Close함수를 호출하여 이값을 반환함.
  - SendBuffer
    - FreeSize (프로퍼티): 남은 버퍼의 크기를 반환함(전체크기 - usedSize)
    - SendBuffer(int chunckSize)(생성자): chunckSize의 바이트 배열을 생성함.
    - Open(int reserveSize): reserveSize > FreeSize이면 return null하고 그렇지 않으면, ArraySegment<byte>(_buffer, _usedSize, reserveSize) 반환함.
    - Close(int usedSize): 원래 usedSize에 이번에 사용한 usedSize를 더함. ArraySegment<byte>(_buffer, _usedSize, usedSize)를 반환함.
  
- Session.cs
   - Session(abstract)
      - Start(Socket socket): receive, send용 SocketAsyncEventArgs에 Completed 이벤트를 연결함.
      - Send(ArraySegment<byte> sendBuff): Send 호출시, sendQueue에 sendBuff를 입력하고, 현재 보내고 있는 작업중이 없으면(_pendinglist.Count == 0) RegiseterSend 함수 호출함.
      - Disconnect(): Conncetor.Connect에서 생성하여 사용하던 socket을 해당 endPoint로 부터 disconnect함.
      - RegisterSend(): sendBuff에서 값을 꺼내어 _pendinglist에 추가하고, 이를 send용 SocketAsyncEventArgs에 bufferlist로 정의함. 연걸된 소켓에 데이터를 보냄. (socket.SendAsync(_sendArgs))
      - OnSendCompleted(object sender, SocketAsyncEventArgs args) : 소켓이 제대로 보내졌으면, bufferlist와 _pendinglist를 null처리함. 그리고 Onsend함수 호출. sendQueue에 값이 있으면(RegisterSend, OnSendCompleted 함수를 처리하는 동안, send함수가 발동하여 큐에 쌓이는 경우)  RegisterSend함수를 호출함.
      - RegisterRecv(): RecvBuffer의 WriteSegment를 호출하여 ArraySegment<byte>를 정의함. receive용 SocketAsyncEventArgs의 Setbuffer에 해당 segment를 채움. 연결된 소켓에서 데이터를 받으면, OnRecvCompleted를 호출함(socket.ReceiveAsync(_recvArgs))
      - OnRecvCompleted(object sender, SocketAsyncEventArgs args) : 소켓이 제대로 받아졌으면, write커서를 이동하고, read커서를 이동함. OnRecv(_recvBuffer.ReadSegment) 호출함. 그리고 RegisterRecv를 호출함.
  
      - OnConnected(EndPoint endPoint)(abstract)
      - OnRecv(ArraySegment<byte> buffer)(abstract)      
      - OnSend(int numOfBytes)(abstract)
      - OnDisconnected(EndPoint endPoint)(abstract)   
  
   - PacketSession: Session(abstract)
      - int OnRecv(ArraySegment<byte> buffer)(sealed override): 헤더의 크기보다 buffer의 크기가 작으면 break. buffer의 offset부터 ushort가져와서 그 값이 buffer.Count보다 작으면 break OnRecvPacket함수를 이용하여 패킷을 조립하고, processLen에 방금 조립한 패킷의 dataSize를 더함. buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize)로 재정의함. processLen 반환
      - void OnRecvPacket(ArraySegment<byte> buffer)(abstract) 

  
  ## 흐름 순서
  Program(DummyClient) -> Connector -> Serversession.Start -> Serversession.OnConnected
