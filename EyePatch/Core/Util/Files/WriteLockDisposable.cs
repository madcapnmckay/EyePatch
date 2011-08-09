using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace EyePatch.Core.Util.Files
{
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim readlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteLockDisposable"/> class.
        /// </summary>
        /// <param name="readlock">The rw lock.</param>
        public WriteLockDisposable(ReaderWriterLockSlim readlock)
        {
            this.readlock = readlock;
            readlock.EnterWriteLock();
        }

        void IDisposable.Dispose()
        {
            readlock.ExitWriteLock();
        }
    }
}