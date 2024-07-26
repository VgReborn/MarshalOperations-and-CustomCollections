using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using VCE.Internal;
using System.Collections.Generic;

#pragma warning disable 0628

namespace VCE
{
    /// <summary>
    /// <para>
    /// A separate version of List. Similar to a list respectively, but this version allows you to directly access the _items without using
    /// reflections to get the array. Goes on par with VCE.MarshalOperations. It is created for performance purposes and will not
    /// not have much flexibility compared to a list, but gets rid of certain isssues
    /// </para>
    /// </summary>
    public sealed class Arr<T> : Collection<T>, IList<T> where T : unmanaged
    {
        /// <summary>
        /// The internal array; The array that the Arr class is wrapped on
        /// </summary>
        public T[] items;
        
        /// <summary>
        /// The actual size of the List, and doesn't count how much of the rows are being used
        /// </summary>
        internal int size;
        /// <summary>
        /// Default value if a size has not been set
        /// </summary>
        static readonly T[] _emptyArray = new T[0];
        private static readonly int _defaultSize = 5;
        
        private readonly int MaxArrayLength = 0x7ffffff;
        /// <summary>
        /// Allow the Arr to change size if needed
        /// </summary>

        // This is created because you may not want to change the size of the list or in this case, Arr
        // but want to keep the functionalities of it
        ///<summary>
        ///Allow whether or not the list should resize if limit is reached. True by default
        ///</summary>
        internal readonly bool dynamic = true;

        public Arr(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("Capacity cannot be less than 0. You need a non negative value");
            Contract.EndContractBlock();
            if (capacity == 0) items = _emptyArray;
            else
                items = new T[capacity];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="dynamic">Make the list automatically change size when needed</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Arr(int capacity, bool dynamic)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("Capacity cannot be less than 0. You need a non negative value");
            Contract.EndContractBlock();
            if (capacity == 0) items = _emptyArray;
            else
                items = new T[capacity];
            this.dynamic = dynamic;
        }

        public Arr() 
            => items = _emptyArray;

        public Arr(T[] a)
        {
            items = new T[a.Length];
            items = a;
        }
        public Arr(T[] a, bool dynamic)
        {
            items = new T[a.Length];
            items = a;
        }



        public Arr(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            ICollection<T> c = (ICollection<T>)collection; //To prevent null values
            if (c != null)
            {
                int count = c.Count;
                if (count == 0)

                    items = _emptyArray;
                else
                {
                    items = new T[count];
                    c.CopyTo(items, 0);
                    size = count;
                }
            }
            else
            {
                size = 0;
                items = _emptyArray;
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext()) Add(en.Current);
                }
            }
        }

        /// <summary>
        /// Gets and sets the capacity of this list.  The capacity is the size of
        /// the internal array used to hold items.  When set, the internal 
        /// array of the list is reallocated to the given capacity.
        /// To sum it up, the size of the list, not the array
        /// </summary>
        // (Not original code. Actual source: https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs)
        public int Capacity
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return items.Length;
            }
            set
            {
                if (value < size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                Contract.EndContractBlock();

                if (value != items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (size > 0)
                        {
                            Array.Copy(items, 0, newItems, 0, size);
                        }
                        items = newItems;
                    }
                    else
                    {
                        items = _emptyArray;
                    }
                }
            }
        }
        //If we manually get the value via varName[value], it would either return the value (unless out of bounds)
        //or set the value (set)
        public new T this[int index]
        {
            get
            {
                if (index > size)
                    throw new IndexOutOfRangeException();
                return items[index];
            }
            set
            {
                if (index > size)
                    throw new IndexOutOfRangeException();
                items[index] = value;
            }
        }

        protected new void Insert(int index, T item)
        {
            if (index > size)
                throw new IndexOutOfRangeException();
            Contract.EndContractBlock();

            if (size == items.Length && dynamic) EnsureCapacity(size + 1);
            if (index < size)
            {
                Array.Copy(items, index, items, index + 1, size - index);
            }
            items[index] = item;
            size++;
        }

        public new void Add(T item)
        {
            if (items.IsReadOnly)
                throw new ReadOnlyException();

            if (size == items.Length && dynamic) EnsureCapacity(size + 1);
            items[size++] = item;
        }
        protected sealed override void ClearItems()
        {
            if (size > 0)
            {
                Array.Clear(items, 0, size);
                size = 0;
            }

        }

        public new int IndexOf(T value)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (Equals(items[i], value))
                    return i;
            }
            return -1;
        }

        public new bool Contains(T value)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (Equals(items[i], value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// This controls the size of the list
        /// </summary>
        private void EnsureCapacity(int min)
        {
            if (items.Length < min && dynamic)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("Called");
#endif

                int newCapacity = items.Length == 0 ? _defaultSize : items.Length * 2;
                if ((uint)newCapacity > MaxArrayLength) newCapacity = MaxArrayLength;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public Span<T> GetSpan()
#if !NET5_0_OR_GREATER
            => MarshalOperations.ConvertToSpan(this);
#else
            => items.AsSpan();
#endif

    }
}
