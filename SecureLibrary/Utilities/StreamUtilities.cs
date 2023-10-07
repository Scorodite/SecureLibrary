using System;
using System.IO;

namespace SecureLibrary.Utilities
{
    public static class StreamUtilities
    {
        public static void Skip(this Stream stream, int count)
        {
            if (stream.CanSeek)
            {
                stream.Seek(count, SeekOrigin.Current);
            }
            else
            {
                byte[] buff = new byte[Math.Min(count, 1024 * 8)];
                int left = count;

                while (left > 0)
                {
                    int read = stream.Read(buff, 0, Math.Min(left, buff.Length));
                    left -= read;

                    if (read == 0)
                    {
                        return;
                    }
                }
            }
        }

        public static void Skip(this BinaryReader reader, int count)
        {
            reader.BaseStream.Skip(count);
        }

        public static int CopyToLimited(this Stream stream, Stream destination, int limit)
        {
            byte[] buff = new byte[Math.Min(limit, 1024 * 8)];
            int left = limit;

            while (left > 0)
            {
                int read = stream.Read(buff, 0, Math.Min(left, buff.Length));
                left -= read;

                if (read == 0)
                {
                    break;
                }

                destination.Write(buff, 0, read);
            }

            return limit - left;
        }
    }
}
