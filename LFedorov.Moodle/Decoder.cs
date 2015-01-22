using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace LFedorov.Moodle
{
    internal class Decoder
    {
        private string m_Headers = "";
        private string m_BoundaryID = "";
        private MemoryStream m_MsgStream;
        private ArrayList m_Entries;

        public string Decode(byte[] msg)
        {
            m_MsgStream = new MemoryStream(msg);
            m_Headers = ParseHeaders(m_MsgStream);
            m_BoundaryID = ParseHeaderFiledSubField("Content-Type:", "boundary", m_Headers);
            return GetHtml();
        }

        private static void GetEntries(ICollection entries, ArrayList allEntries)
        {
            if (entries == null) return;

            allEntries.AddRange(entries);

            foreach (Entry entry in entries)
            {
                GetEntries(entry.MimeEntries, allEntries);
            }
        }

        private ArrayList Entries
        {
            get
            {
                m_Entries = ParseEntries(m_MsgStream, m_Headers.Length, m_BoundaryID);
                return m_Entries;
            }
        }

        private string GetHtml()
        {
            m_Entries = ParseEntries(m_MsgStream, m_Headers.Length, m_BoundaryID);

            // Find first text entry
            var entries = new ArrayList();
            GetEntries(Entries, entries);

            foreach (var entry in entries.Cast<Entry>().Where(entry => entry.ContentType.ToUpper().IndexOf("TEXT/HTML", StringComparison.Ordinal) > -1))
            {
                return entry.DataS;
            }

            return "";
        }

        internal static string ParseHeaders(Stream entryStrm)
        {
            var crlf = new[] { (byte)'\r', (byte)'\n' };
            var msHeaders = new MemoryStream();
            var r = new Reader(entryStrm);
            var lineData = r.ReadLine();
            while (lineData != null)
            {
                if (lineData.Length == 0)
                {
                    break;
                }

                msHeaders.Write(lineData, 0, lineData.Length);
                msHeaders.Write(crlf, 0, crlf.Length);
                lineData = r.ReadLine();
            }

            return Encoding.Default.GetString(msHeaders.ToArray());
        }

        internal static string ParseHeaderField(string fieldName, string headers)
        {
            using (TextReader r = new StreamReader(new MemoryStream(Encoding.Default.GetBytes(headers))))
            {
                var line = r.ReadLine();
                while (line != null)
                {
                    // Find line where field begins
                    if (line.ToUpper().StartsWith(fieldName.ToUpper()))
                    {
                        // Remove field name and start reading value
                        string fieldValue = line.Substring(fieldName.Length).Trim();

                        // see if multi line value. See commnt above.
                        line = r.ReadLine();
                        while (line != null && (line.StartsWith("\t") || line.StartsWith(" ")))
                        {
                            fieldValue += line;
                            line = r.ReadLine();
                        }

                        return fieldValue;
                    }

                    line = r.ReadLine();
                }
            }

            return "";
        }

        internal string ParseContentType(string headers)
        {
            var contentType = ParseHeaderField("CONTENT-TYPE:", headers);
            return contentType.Length > 0 ? contentType.Split(';')[0] : "text/plain";
        }

        internal ArrayList ParseEntries(MemoryStream msgStrm, int pos, string boundaryID)
        {
            if (m_Entries != null) return m_Entries;

            var entries = new ArrayList();
            var contentType = ParseContentType(m_Headers);

            if (contentType.ToLower().IndexOf("multipart/", StringComparison.Ordinal) == -1)
            {
                entries.Add(new Entry(msgStrm.ToArray(), this));
                m_Entries = entries;

                return m_Entries;
            }

            msgStrm.Position = pos;

            if (boundaryID.Length > 0)
            {
                var strmEntry = new MemoryStream();
                var reader = new Reader(msgStrm);
                var lineData = reader.ReadLine();

                // Search first entry
                while (lineData != null)
                {
                    var line = Encoding.Default.GetString(lineData);
                    if (line.StartsWith("--" + boundaryID))
                    {
                        break;
                    }

                    lineData = reader.ReadLine();
                }

                // Start reading entries
                while (lineData != null)
                {
                    // Read entry data
                    var line = Encoding.Default.GetString(lineData);
                    // Next boundary
                    if (line.StartsWith("--" + boundaryID) && strmEntry.Length > 0)
                    {
                        // Add Entry
                        entries.Add(new Entry(strmEntry.ToArray(), this));

                        strmEntry.SetLength(0);
                    }
                    else
                    {
                        strmEntry.Write(lineData, 0, lineData.Length);
                        strmEntry.Write(new[] { (byte)'\r', (byte)'\n' }, 0, 2);
                    }

                    lineData = reader.ReadLine();
                }
            }

            return entries;
        }

        internal static string ParseHeaderFiledSubField(string fieldName, string subFieldName, string headers)
        {
            var mainFiled = ParseHeaderField(fieldName, headers);
            // GetContent sub field value
            if (mainFiled.Length > 0)
            {
                var index = mainFiled.ToUpper().IndexOf(subFieldName.ToUpper(), StringComparison.Ordinal);
                if (index > -1)
                {
                    mainFiled = mainFiled.Substring(index + subFieldName.Length + 1); // Remove "subFieldName="

                    // subFieldName value may be in "" and without
                    if (mainFiled.StartsWith("\""))
                    {
                        return mainFiled.Substring(1, mainFiled.IndexOf("\"", 1, StringComparison.Ordinal) - 1);
                    }
                    // value without ""
                    var endIndex = mainFiled.Length;
                    if (mainFiled.IndexOf(" ", StringComparison.Ordinal) > -1)
                    {
                        endIndex = mainFiled.IndexOf(" ", StringComparison.Ordinal);
                    }

                    return mainFiled.Substring(0, endIndex);
                }
            }

            return "";
        }
    }
}