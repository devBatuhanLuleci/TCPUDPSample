using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TCPUDPSample.Core
{
    public class Packet : IDisposable
    {
        private List<byte> bufferList;
        private byte[]? bufferArray;
        private int readPosition = 0;
        private bool disposed = false;

        public Packet()
        {
            bufferList = new List<byte>();
            bufferArray = null;
            readPosition = 0;
        }

        public Packet(int _id)
        {
            bufferList = new List<byte>();
            bufferArray = null;
            readPosition = 0;

            Write(_id);
        }

        public Packet(byte[] _data)
        {
            bufferList = new List<byte>();
            bufferArray = null;
            readPosition = 0;

            SetBytes(_data);
        }

        public void SetBytes(byte[] _data)
        {
            Write(_data);
            bufferArray = bufferList.ToArray();
        }

        public void Write(byte _value)
        {
            bufferList.Add(_value);
        }

        public void Write(byte[] _value)
        {
            bufferList.AddRange(_value);
        }

        public void Write(short _value)
        {
            bufferList.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(int _value)
        {
            bufferList.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(long _value)
        {
            bufferList.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(float _value)
        {
            bufferList.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(bool _value)
        {
            bufferList.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(double _value)
        {
            bufferList.AddRange(BitConverter.GetBytes(_value));
        }

        public void Write(string _value)
        {
            Write(_value.Length);
            Write(Encoding.UTF8.GetBytes(_value));
        }

        public void WriteLength()
        {
            bufferList.InsertRange(0, BitConverter.GetBytes(bufferList.Count));
        }

        public void InsertInt(int _value)
        {
            bufferList.InsertRange(0, BitConverter.GetBytes(_value));
        }

        public byte[] ToArray()
        {
            bufferArray = bufferList.ToArray();
            return bufferArray;
        }

        public int Length()
        {
            return bufferList.Count;
        }

        public int UnreadLength()
        {
            return Length() - readPosition;
        }

        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                bufferList.Clear();
                bufferArray = null;
                readPosition = 0;
            }
            else
            {
                readPosition -= 4; // "Unread" the last int, for example, or common logic
            }
        }

        public byte ReadByte(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                byte _value = bufferArray![readPosition];
                if (_moveReadPos)
                {
                    readPosition++;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                byte[] _value = bufferList.GetRange(readPosition, _length).ToArray();
                if (_moveReadPos)
                {
                    readPosition += _length;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        public short ReadShort(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                short _value = BitConverter.ToInt16(bufferArray!, readPosition);
                if (_moveReadPos)
                {
                    readPosition += 2;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        public int ReadInt(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                int _value = BitConverter.ToInt32(bufferArray!, readPosition);
                if (_moveReadPos)
                {
                    readPosition += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        public long ReadLong(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                long _value = BitConverter.ToInt64(bufferArray!, readPosition);
                if (_moveReadPos)
                {
                    readPosition += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        public float ReadFloat(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                float _value = BitConverter.ToSingle(bufferArray!, readPosition);
                if (_moveReadPos)
                {
                    readPosition += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        public double ReadDouble(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                double _value = BitConverter.ToDouble(bufferArray!, readPosition);
                if (_moveReadPos)
                {
                    readPosition += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'double'!");
            }
        }

        public bool ReadBool(bool _moveReadPos = true)
        {
            if (bufferList.Count > readPosition)
            {
                bool _value = BitConverter.ToBoolean(bufferArray!, readPosition);
                if (_moveReadPos)
                {
                    readPosition += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt();
                string _value = Encoding.UTF8.GetString(bufferArray!, readPosition, _length);
                if (_moveReadPos && _length > 0)
                {
                    readPosition += _length;
                }
                return _value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            bufferList.Clear();
            bufferArray = null;
        }
    }
}
