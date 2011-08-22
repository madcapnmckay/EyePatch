using System;
using System.Threading;

namespace EyePatch.Core.Util.Files
{
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim readlock;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "WriteLockDisposable" /> class.
        /// </summary>
        /// <param name = "readlock">The rw lock.</param>
        public WriteLockDisposable(ReaderWriterLockSlim readlock)
        {
            this.readlock = readlock;
            readlock.EnterWriteLock();
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            readlock.ExitWriteLock();
        }

        #endregion
    }
}