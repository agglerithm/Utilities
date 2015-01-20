using System.IO;

namespace Utilities.IO
{
    public interface IParseFiles<T> where T:FileRecordBase, new()
    {
        IRecordBlock<T> AnalyzeFile(Stream readStream);
    }
}