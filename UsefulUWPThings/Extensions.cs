using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace UsefulUWPThings
{
    public static class Extensions
    {
        public static async Task<byte[]> ToByteArray(this InMemoryRandomAccessStream stream)
        {
            return await Streams.StreamToByteArray(stream);
        }
    }
}
