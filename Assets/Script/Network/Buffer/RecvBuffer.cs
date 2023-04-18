using System;


namespace NetworkCore
{
    public class RecvBuffer
    {

        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;
        //������ġ- ������ ���� ��ġ
        public int DataSize { get { return _writePos - _readPos; } }
        //�ѻ����� - ������ġ
        public int FreeSize { get { return _buffer.Count - _writePos; } }


        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }
        public ArraySegment<byte> ReadSegment
        {
            //[0].....[_readPos] => 0���� _readPos��ŭ ��ȯ
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            //�ѹ��ۿ��� , ������ġ , 
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
            {
                return false;
            }

            _readPos += numOfBytes;
            return true;

        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
            {
                return false;
            }

            _writePos += numOfBytes;
            return true;
        }


        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }


    }
}

