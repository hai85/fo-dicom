﻿// Copyright (c) 2012-2015 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using System;
using System.IO;

namespace Dicom.IO
{
    public class FileByteTarget : IDisposable, IByteTarget
    {
        private IFileReference _file;

        private Stream _stream;

        private Endian _endian;

        private BinaryWriter _writer;

        private object _lock;

        public FileByteTarget(IFileReference file)
        {
            _file = file;
            _stream = _file.OpenWrite();
            _endian = Endian.LocalMachine;
            _writer = EndianBinaryWriter.Create(_stream, _endian);
            _lock = new object();
        }

        public Endian Endian
        {
            get
            {
                return _endian;
            }
            set
            {
                if (_endian != value)
                {
                    lock (_lock)
                    {
                        _endian = value;
                        _writer = EndianBinaryWriter.Create(_stream, _endian);
                    }
                }
            }
        }

        public long Position
        {
            get
            {
                return _stream.Position;
            }
        }

        public void Write(byte v)
        {
            _writer.Write(v);
        }

        public void Write(short v)
        {
            _writer.Write(v);
        }

        public void Write(ushort v)
        {
            _writer.Write(v);
        }

        public void Write(int v)
        {
            _writer.Write(v);
        }

        public void Write(uint v)
        {
            _writer.Write(v);
        }

        public void Write(long v)
        {
            _writer.Write(v);
        }

        public void Write(ulong v)
        {
            _writer.Write(v);
        }

        public void Write(float v)
        {
            _writer.Write(v);
        }

        public void Write(double v)
        {
            _writer.Write(v);
        }

        public void Write(
            byte[] buffer,
            uint offset = 0,
            uint count = uint.MaxValue,
            ByteTargetCallback callback = null,
            object state = null)
        {
            if (count == uint.MaxValue) count = (uint)buffer.Length - offset;

            if (callback != null)
                _stream.BeginWrite(
                    buffer,
                    (int)offset,
                    (int)count,
                    OnEndWrite,
                    new Tuple<ByteTargetCallback, object>(callback, state));
            else _stream.Write(buffer, (int)offset, (int)count);
        }

        private void OnEndWrite(IAsyncResult result)
        {
            try
            {
                _stream.EndWrite(result);
            }
            catch
            {
            }
            finally
            {
                if (result.AsyncState != null)
                {
                    Tuple<ByteTargetCallback, object> state = result.AsyncState as Tuple<ByteTargetCallback, object>;
                    state.Item1(this, state.Item2);
                }
            }
        }

        public void Close()
        {
            try
            {
                _stream.Close();
                _stream = null;
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
