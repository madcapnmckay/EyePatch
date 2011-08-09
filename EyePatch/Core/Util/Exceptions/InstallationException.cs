using System;

namespace EyePatch.Core.Util.Exceptions
{
    public class InstallationException : ApplicationException
    {
        public object InstallData { get; protected set; }

        public int ErrorCode { get; protected set; }

        public InstallationException(string message, int code) : this(message, code, null) {}

        public InstallationException(string message, int code, object data) : base(message)
        {
            ErrorCode = code;
            InstallData = data;
        }
    }
}