using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace UsefulUWPThings
{
    public static class Streams
    {
        public static async Task<InMemoryRandomAccessStream> ByteArrayToStream(byte[] data)
        {
            var stream = new InMemoryRandomAccessStream();
            using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
            {
                writer.WriteBytes(data);
                await writer.StoreAsync();
                writer.DetachStream();
            }
            return stream;
        }

        public static async Task<byte[]> StreamToByteArray(IRandomAccessStream stream)
        {
            byte[] bytes = new byte[stream.Size];

            using (DataReader reader = new DataReader(stream.GetInputStreamAt(0)))
            {
                await reader.LoadAsync((uint)bytes.Length);
                reader.ReadBytes(bytes);
                reader.DetachStream();
            }

            return bytes;
        }
    }
}
