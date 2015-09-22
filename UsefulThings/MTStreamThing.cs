using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    /// <summary>
    /// Threadsafe collection of bytes that can grow. Basically a ConcurrentBag, but with more atomicity. Also stream-like ability to write to random areas beyond length of list.
    /// e.g. Write operations expand list as required, then write to that location in one operation.
    /// </summary>
    [Obsolete("Just don't use this. It doesn't work, but it might show promise later and I don't want to have to rewrite it")]
    public class MTStreamThing
    {
        /// <summary>
        /// Indicates whether write operations are allowed.
        /// Usually disabled as multi-threaded writing can be painful, and I want you to be damn sure you want to do it.
        /// </summary>
        public bool CanWrite = false;

        /// <summary>
        /// Current length of collection.
        /// </summary>
        public int Length
        {
            get
            {
                lock (locker)
                    return backingStore.Count;
            }
        }

        List<byte> backingStore = null;
        readonly Object locker = new Object();

        #region Constructors
        /// <summary>
        /// Initialises threadsafe, seekable collection of bytes.
        /// </summary>
        /// <param name="canWrite">True = writing is allowed.</param>
        public MTStreamThing(bool canWrite = false)
        {
            backingStore = new List<byte>();
            CanWrite = canWrite;
        }


        /// <summary>
        /// Initialises threadsafe, seekable collection with an initial length.
        /// </summary>
        /// <param name="length">Length to start with.</param>
        /// <param name="canWrite">True = writing is allowed.</param>
        public MTStreamThing(int length, bool canWrite = false)
        {
            backingStore = new List<byte>(length);
            CanWrite = canWrite;
        }


        /// <summary>
        /// Initialises threadsafe, seekable collection containing the given data.
        /// </summary>
        /// <param name="data">Initialising data.</param>
        /// <param name="canWrite">True = writing is allowed.</param>
        public MTStreamThing(IEnumerable<byte> data, bool canWrite = false)
        {
            backingStore = new List<byte>(data);
            CanWrite = canWrite;
        }
        #endregion Constructors


        /// <summary>
        /// Writes item to the end of this stream-thing .
        /// </summary>
        /// <param name="item">Item to write.</param>
        public void Write(byte item)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is currently read only. Change Operation mode to write.");

            lock(locker)
                backingStore.Add(item);
        }


        /// <summary>
        /// Writes item to this stream-thing at the given absolute offset.
        /// </summary>
        /// <param name="item">Item to write.</param>
        /// <param name="offset">Index to write to.</param>
        public void Write(byte item, int offset)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is currently read only. Change Operation mode to write.");

            CheckSize(offset);
            backingStore[offset] = item;
        }


        /// <summary>
        /// Writes collection of items to end of this stream-thing.
        /// </summary>
        /// <param name="items">Items to write.</param>
        /// <returns>Number of items written.</returns>
        public int Write(IEnumerable<byte> items)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is currently read only. Change Operation mode to write.");

            lock (locker)
                backingStore.AddRange(items);

            return items.Count();
        }


        /// <summary>
        /// Writes collection to this stream-thing at the given offset.
        /// </summary>
        /// <param name="items">Items to write.</param>
        /// <param name="offset">Index to write to.</param>
        public void Write(IEnumerable<byte> items, int offset)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is currently read only. Change Operation mode to write.");

            lock (locker)
            {
                CheckSize(offset);
                int count = offset;
                foreach (var item in items)
                    backingStore[count++] = item;
            }
        }


        /// <summary>
        /// Writes items from another stream-thing to the end of this stream-thing.
        /// </summary>
        /// <param name="items">Items to write.</param>
        public void Write(MTStreamThing items)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is currently read only. Change Operation mode to write.");

            lock (locker)
                backingStore.AddRange(items.backingStore);
        }


        /// <summary>
        /// Writes items from another stream-thing to specified index of this stream-thing.
        /// </summary>
        /// <param name="items">Items to write.</param>
        /// <param name="offset">Index in this stream-thing to write to.</param>
        public void Write(MTStreamThing items, int offset)
        {
            if (!CanWrite)
                throw new InvalidOperationException("Stream is currently read only. Change Operation mode to write.");

            lock (locker)
            {
                CheckSize(offset);
                backingStore.AddRange(items.backingStore);
            }
        }



        /// <summary>
        /// Reads item from given offset.
        /// </summary>
        /// <param name="offset">Index to read from.</param>
        /// <returns>Item read.</returns>
        public byte Read(int offset)
        {
            return backingStore[offset];
        }


        /// <summary>
        /// Reads number of items from specified offset.
        /// </summary>
        /// <param name="offset">Index to start reading from.</param>
        /// <param name="length">Number of items to read.</param>
        /// <returns>Items read.</returns>
        public List<byte> Read(int offset, int length)
        {
            return backingStore.GetRange(offset, length);
        }


        /// <summary>
        /// Checks size of collection and expands as required, filling empty space with the default value of the item.
        /// </summary>
        /// <param name="desiredOffset">Index in list required.</param>
        private void CheckSize(int desiredOffset)
        {
            lock (locker)
            {
                Debugger.Break();
                int difference = desiredOffset - backingStore.Count;
                if (difference >= 0)
                    backingStore.AddRange(Enumerable.Repeat((byte)0, difference));
            }
        }


        #region ReadInt functions
        /// <summary>
        /// Reads two bytes from the collection at the given position and bitshifts them to an unsigned 16 bit integer.
        /// </summary>
        /// <param name="Position">Index to start reading from.</param>
        /// <returns>ushort read from stream.</returns>
        public ushort ReadInt16(int Position)
        {
            return (ushort)(backingStore[Position] & (backingStore[Position + 1] << 8));
        }


        /// <summary>
        /// Reads four bytes from the collection at the given position and bitshifts them to an unsigned 32 bit integer.
        /// </summary>
        /// <param name="Position">Index to start reading from.</param>
        /// <returns>32 bit unsigned integer.</returns>
        public uint ReadInt32(int Position)
        {
            return (uint)(
                backingStore[Position] &
                (backingStore[Position + 1] << 8) &
                (backingStore[Position + 2] << 16) &
                (backingStore[Position + 3] << 24)
                );
        }
        #endregion
    }
}
