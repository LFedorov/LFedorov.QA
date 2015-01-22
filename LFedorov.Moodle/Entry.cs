using System;
using System.Collections;
using System.IO;
using System.Text;

namespace LFedorov.Moodle
{
    internal class Entry
    {
        private readonly string m_ContentType = "";
        private readonly string m_CharSet = "";
        private readonly string m_ContentEncoding = "";
        private readonly byte[] m_Data;
        private readonly ArrayList m_Entries;

        public Entry(byte[] mimeEntry, Decoder mime)
        {
            var entryStrm = new MemoryStream(mimeEntry);
            var headers = Decoder.ParseHeaders(entryStrm);

            m_ContentType = mime.ParseContentType(headers);
            m_Entries = new ArrayList();

            // != multipart content
            if (m_ContentType.ToLower().IndexOf("multipart", StringComparison.Ordinal) == -1)
            {
                m_CharSet = ParseCharSet(headers);

                var encoding = Decoder.ParseHeaderField("CONTENT-TRANSFER-ENCODING:", headers);
                m_ContentEncoding = encoding.Length > 0 ? encoding : "7bit";

                m_Data = new byte[entryStrm.Length - entryStrm.Position];
                entryStrm.Read(m_Data, 0, m_Data.Length);
            }
            // multipart content, get nested entries
            else
            {
                long s = (int)entryStrm.Position;
                var boundaryID = Decoder.ParseHeaderFiledSubField("Content-Type:", "boundary", headers);
                m_Entries = mime.ParseEntries(entryStrm, (int)entryStrm.Position, boundaryID);

                entryStrm.Position = s;
                m_Data = new byte[entryStrm.Length - s];
                entryStrm.Read(m_Data, 0, m_Data.Length);
            }
        }

        private static string ParseCharSet(string headers)
        {
            var charset = Decoder.ParseHeaderFiledSubField("Content-Type:", "charset", headers);

            if (charset.EndsWith(";"))
            {
                charset = charset.Substring(0, charset.Length - 1);
            }

            if (charset.Length <= 0) return "ascii";

            try
            {
                return charset;
            }
            catch
            {
                return "ascii";
            }
        }

        public ArrayList MimeEntries
        {
            get { return m_Entries; }
        }

        public string ContentType
        {
            get { return m_ContentType; }
        }


        public byte[] Data
        {
            get
            {
                switch (m_ContentEncoding.ToLower())
                {
                    case "quoted-printable":
                        return QuotedPrintableDecodeB(m_Data, m_ContentType.ToLower().IndexOf("text", StringComparison.Ordinal) > -1);
                    case "base64":
                        var dataStr = Encoding.Default.GetString(m_Data);
                        return dataStr.Trim().Length > 0 ? Convert.FromBase64String(dataStr) : new byte[] { };
                    default:
                        return m_Data;
                }
            }
        }

        public string DataS
        {
            get
            {
                try
                {
                    return Encoding.GetEncoding(m_CharSet).GetString(Data);
                }
                catch
                {
                    return Encoding.Default.GetString(Data);
                }
            }
        }

        public static byte[] FromHex(byte[] hexData)
        {
            if (hexData.Length < 2 || (hexData.Length / (double)2 != Math.Floor(hexData.Length / (double)2)))
            {
                throw new Exception("Illegal hex data, hex data must be in two bytes pairs, for example: 0F,FF,A3,... .");
            }

            var retVal = new MemoryStream(hexData.Length / 2);
            // Loop hex value pairs
            for (var i = 0; i < hexData.Length; i += 2)
            {
                var hexPairInDecimal = new byte[2];
                // We need to convert hex char to decimal number, for example F = 15
                for (var h = 0; h < 2; h++)
                {
                    switch (((char)hexData[i + h]))
                    {
                        case '0':
                            hexPairInDecimal[h] = 0;
                            break;
                        case '1':
                            hexPairInDecimal[h] = 1;
                            break;
                        case '2':
                            hexPairInDecimal[h] = 2;
                            break;
                        case '3':
                            hexPairInDecimal[h] = 3;
                            break;
                        case '4':
                            hexPairInDecimal[h] = 4;
                            break;
                        case '5':
                            hexPairInDecimal[h] = 5;
                            break;
                        case '6':
                            hexPairInDecimal[h] = 6;
                            break;
                        case '7':
                            hexPairInDecimal[h] = 7;
                            break;
                        case '8':
                            hexPairInDecimal[h] = 8;
                            break;
                        case '9':
                            hexPairInDecimal[h] = 9;
                            break;
                        case 'a':
                        case 'A':
                            hexPairInDecimal[h] = 10;
                            break;
                        case 'b':
                        case 'B':
                            hexPairInDecimal[h] = 11;
                            break;
                        case 'c':
                        case 'C':
                            hexPairInDecimal[h] = 12;
                            break;
                        case 'd':
                        case 'D':
                            hexPairInDecimal[h] = 13;
                            break;
                        case 'e':
                        case 'E':
                            hexPairInDecimal[h] = 14;
                            break;
                        case 'f':
                        case 'F':
                            hexPairInDecimal[h] = 15;
                            break;
                    }
                }

                // Join hex 4 bit(left hex cahr) + 4bit(right hex char) in bytes 8 it
                retVal.WriteByte((byte)((hexPairInDecimal[0] << 4) | hexPairInDecimal[1]));
            }

            return retVal.ToArray();
        }
        public static byte[] QuotedPrintableDecodeB(byte[] data, bool includeCRLF)
        {
            var strm = new MemoryStream(data);
            var dStrm = new MemoryStream();

            var b = strm.ReadByte();
            while (b > -1)
            {
                // Hex eg. =E4
                if (b == '=')
                {
                    var buf = new byte[2];
                    strm.Read(buf, 0, 2);

                    // <CRLF> followed by =, it's splitted line
                    if (!(buf[0] == '\r' && buf[1] == '\n'))
                    {
                        try
                        {
                            var convertedByte = FromHex(buf);
                            dStrm.Write(convertedByte, 0, convertedByte.Length);
                        }
                        catch (Exception)
                        { // If worng hex value, just skip this chars							
                        }
                    }
                }
                else
                {
                    // For text line breaks are included, for binary data they are excluded

                    if (includeCRLF)
                    {
                        dStrm.WriteByte((byte)b);
                    }
                    else
                    {
                        // Skip \r\n they must be escaped
                        if (b != '\r' && b != '\n')
                        {
                            dStrm.WriteByte((byte)b);
                        }
                    }
                }

                b = strm.ReadByte();
            }

            return dStrm.ToArray();
        }
    }
}