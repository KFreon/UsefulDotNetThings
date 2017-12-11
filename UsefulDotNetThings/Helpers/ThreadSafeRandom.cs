using System;
using System.Threading;

namespace UsefulDotNetThings.Helpers
{
    /// <summary>
    /// Provides access to a threadsafe random. 
    /// I dunno why this really exists. Used by shuffle.
    /// </summary>
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        /// <summary>
        /// Gets a threadsafe random.
        /// </summary>
        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
