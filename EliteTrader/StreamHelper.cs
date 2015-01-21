using System.IO;

namespace EliteTrader
{
    public static class StreamHelper
    {
        private const int MAX_BYTES_TO_READ = 504857600; //500MB

        public static byte[] ReadAll(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            long streamLength = stream.Length;
            if (streamLength > MAX_BYTES_TO_READ)
            {
                throw new IOException(string.Format("Unable to read the whole requested stream. The size of the stream ({0}) is larger than the maximum allowed size ({1}).", streamLength, MAX_BYTES_TO_READ));
            }

            byte[] bytes = Read(stream, (int)stream.Length);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return bytes;
        }

        public static byte[] Read(Stream stream, int numberOfBytesToRead)
        {
            byte[] buffer = new byte[numberOfBytesToRead];

            int remaining = numberOfBytesToRead;
            int offset = 0;
            while (remaining > 0)
            {
                int bytesRead = stream.Read(buffer, offset, remaining);
                if (bytesRead == 0)
                {
                    throw new IOException("Unable to read the requested number of bytes from the stream. The end of the stream was reached before expected.");
                }
                remaining -= bytesRead;
                offset += bytesRead;
            }
            return buffer;
        }
    }
}
