# GameServer
[C#과 유니티로 만드는 MMORPG 게임 개발 시리즈] Part4: 게임 서버

### 소켓 프로그래밍 입문
- 간단한 클라이언트, 서버 스크립트를 작성했다.
- 블록킹함수인 Connect, Receive, Send를 사용하였다는 점을 개선하여야한다.

### Listener
- Accept를 넌블로킹함수를 사용하여 개선하였다.
- 의문점 
아래코드를 실행하면 args가 클라이언트에서 보낸 소켓에 대한 정보를 담게 되는건가?
bool pending = _listenSocket.AcceptAsync(args);

### Session
- ReceiveAsync
- SendAsync


## DummyClient
- Program.cs
- ServerSession.cs
## Server
- Program.cs
- ClientSession.cs
## ServerCore
- Conncetor.cs
- Listener.cs
- RecvBuffer.cs
- SendBuffer.cs
- Session.cs
