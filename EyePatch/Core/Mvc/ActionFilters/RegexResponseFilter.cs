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
            get { return 0; }
        }

        private long _position;
        public override long Position
        {
            get { return _position; }
            set { _position = value; }
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

        public override void Close()
        {
            response.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // filter the string
            string html = Encoding.Default.GetString(buffer);

            //remove whitespace
            html = regex.Replace(html, replacement);

            byte[] outdata = Encoding.Default.GetBytes(html);

            response.Write(outdata, 0, outdata.GetLength(0));
        }
    }
}