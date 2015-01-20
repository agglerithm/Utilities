using System;
namespace Utilities.IO
{
    public interface IRecordBlock<T>
     where T : FileRecordBase, new()
    {
        int ActualRecordCount { get; }
        int BlockSize { get; }
        int Offset { get; }
        T[] Read(Func<System.IO.Stream> read);
        System.IO.Stream Read(Func<System.IO.Stream> read, Action<T> process); 
        System.IO.Stream Write(Func<byte[], System.IO.Stream> write);
    }
}
