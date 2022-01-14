using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        //Init이 완료되면(클라이언트에서 Connect연결 요청을 받으면), 이를 ServerCore의 Program에 알려준다.
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler) 
        {
            //문지기
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            //문지기 교육
            _listenSocket.Bind(endPoint);

            //영업 시작
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            // 델리게이트인 EventHandler와 함수 OnAcceptCompleted의 형식을 맞췆어야한다.
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            //손님을 입장시킨다
            //_listenSocket.AcceptAsync(args)에서 args에 대한 이벤트가 성공하면
            //Completed가 실행된다.
            //아래코드를 실행하면 args가 클라이언트에서 보낸 소켓에 대한 정보를 담게 되는건가?
            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args) 
        {
            if (args.SocketError == SocketError.Success)
            {
                //TODO
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);
        }

    }
}
