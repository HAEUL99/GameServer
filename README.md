# GameServer
[C#과 유니티로 만드는 MMORPG 게임 개발 시리즈] Part4: 게임 서버
https://www.inflearn.com/course/%EC%9C%A0%EB%8B%88%ED%8B%B0-mmorpg-%EA%B0%9C%EB%B0%9C-part4

## PacketGenerator
- PacketFormat.cs:  packet의 변수, read, write 자동화 포맷
- PDL.xml: packet 정의
- Program.cs: PacketFormat을 이용하여 GenPacket, ClientPacketManager, ServerPacketManager 생성

## Common
- GenPackets.bat: Program에서 생성한 스크립트를 특정경로로 복제 해줌. 

 - PacketGenerator 출력 경로 바꾸기: 프로젝트 속성> 구성(모든 구성) > 출력 경로(bin\)으로 설정
 - netcoreapp3.1 폴더 생성 안하는 방법: PacektGenerator.csproj에서 <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
 - 배치파일 생성
    - PacketGenerator.exe파일을 눌러주는 역할, PDL.xml 인자로 넣어줌(배치파일 위치 기준으로 경로 설정) : START ../../PacketGenerator/bin/PacketGenerator.exe  ../../PacketGenerator/PDL.xml
    
    - 위코드로 PacketGenerator.exe경로에 만들어진 GenPackets.cs를 DummyClient(Server)/Packet로 복사함: XCOPY /Y GenPackets.cs "../../DummyClient/Packet", XCOPY /Y GenPackets.cs "../../Server/Packet"

## DummyClient
- Program.cs:  호스트연결, connect
- ServerSession: packet클래스에 대해 connect (Session 상속받음.)
- Packet:
    - ClientPacketManager: 받은 packet을 id 기준으로 찾아서 PacketHandler의 해당 함수 invoke.
    - PacketHandler: 각 packet에 대해 recv시 작동함수 작성.(여기서는 cw)


## Server
- Program.cs:  호스트연결, listener
- ClientSession: OnRecvPacket 함수 호출해서, PacketManager의 action.invoke
- Packet:
    - ServerPacketManager: 받은 packet을 id 기준으로 찾아서 PacketHandler의 해당 함수 invoke.
    - PacketHandler: 각 packet에 대해 recv시 작동함수 작성.(여기서는 cw)

## ServerCore
- Connector: connect 소켓 등록
- Listener: socket의 accept을 등록
- RecvBuffer:  데이터 받을때, segment 관리
- SendBuffer: 데이터 보낼때, segment 관리
- Session: 
    - Session: 네트워크 통신(connect, recv, send, disconnected) 
    - PacketSession: Recv 작동에 대한 세부 사항이 있음.
