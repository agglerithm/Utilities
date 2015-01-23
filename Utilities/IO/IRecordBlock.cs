using System;
using System.IO;

namespace Utilities.IO
{
    public interface IRecordBlock<T>
     where T : FileRecordBase, new()
    {
        int ActualRecordCount { get; }
        int BlockSize { get; }
        int Offset { get; }
        T[] Read(Func<Stream> read);
        Stream Read(Func<Stream> read, Action<T> process); 
        Stream Write(Func<byte[], Stream> write);
        void StreamlessWrite(Stream readStream, Action<Stream, int, int> write);
    }
}
