using System.IO;

namespace LFedorov.Moodle
{
    internal class Reader
    {
        private readonly Stream m_StrmSource;

        public Reader(Stream strmSource)
        {
            m_StrmSource = strmSource;
        }

        public byte[] ReadLine()
        {
            var strmLineBuf = new MemoryStream();
            byte prevByte = 0;

            var currByteInt = m_StrmSource.ReadByte();
            while (currByteInt > -1)
            {
                strmLineBuf.WriteByte((byte)currByteInt);

                // Line found
                if ((prevByte == (byte)'\r' && (byte)currByteInt == (byte)'\n'))
                {
                    strmLineBuf.SetLength(strmLineBuf.Length - 2); // Remove <CRLF>

                    return strmLineBuf.ToArray();
                }

                // Store byte
                prevByte = (byte)currByteInt;

                // Read next byte
                currByteInt = m_StrmSource.ReadByte();
            }

            // Line isn't terminated with <CRLF> and has some bytes left, return them.
            return strmLineBuf.Length > 0 ? strmLineBuf.ToArray() : null;
        }
    }
}