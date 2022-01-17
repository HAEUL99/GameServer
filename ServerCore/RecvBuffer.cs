using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class RecvBuffer
    {
        // [][][][][][][][][][]
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize) 
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        //writePos - readPos (유효데이터의 크기)
        public int DataSize { get { return _writePos - _readPos; } }
        // 남은 데이터의 크기
        public int FreeSize { get { return _buffer.Count - _writePos; } }
        

        //어디부터 데이터를 읽으면 되냐 (r ~ w)
        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset+_readPos, DataSize); }
        }
        
        //어디부터 쓰면되냐(w ~)
        public ArraySegment<byte> WriteSegment 
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset+ _writePos, FreeSize); }
        }
        
        // 앞에 공간이 있으면 rw땡겨온다
        public void Clean() 
        {
            int dataSize = DataSize;
            if (dataSize == 0) 
            {
                //남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                // [][][][][rw][][][][][]
                _readPos = _writePos = 0;

            }
            else 
            {
                //남은 찌끄레기가 있으면 시작 위치로 복사
                // [][][r][][w][][][][][]
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        // 성공적으로 read하면 커서 위치를 바꿔줌
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;

        }

        // 성공적으로 write하면 커서 위치를 바꿔줌
        public bool OnWrite(int numOfBytes) 
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }

    }
}
