using System.IO;
using System.IO.Compression;
using System.Text;

namespace BugzNet.Core
{
    public static class CompressUtility
    {
        public static byte[] Zip(string uncompressedString)
        {
            byte[] compressedBytes;
            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }
                    compressedBytes = compressedStream.ToArray();
                }
            }
            return compressedBytes;
        }

        public static byte[] Unzip(byte[] compressedBytes)
        {
            byte[] decompressedBytes;
            var compressedStream = new MemoryStream(compressedBytes);
            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);
                    decompressedBytes = decompressedStream.ToArray();
                }
            }
            return decompressedBytes;
        }
    }
}
