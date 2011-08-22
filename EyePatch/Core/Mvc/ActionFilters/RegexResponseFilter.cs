using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class RegexResponseFilter : Stream
    {
        protected Regex regex;
        protected string replacement;
        protected Stream response;

        public RegexResponseFilter(Stream response, Regex regex, string replacement)
        {
            this.regex = regex;
            this.replacement = replacement;
            this.response = response;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return response.Length; }
        }

        public override long Position
        {
            get { return response.Position; }
            set { response.Position = value; }
        }

        public override void Flush()
        {
            response.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return response.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            response.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return response.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // capture the data and convert to string
            var data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);

            // filter the string
            var s = Encoding.Default.GetString(buffer);
            s = regex.Replace(s, replacement);

            // write the data to stream 
            var outdata = Encoding.Default.GetBytes(s);
            response.Write(outdata, 0, outdata.GetLength(0));
        }
    }
}