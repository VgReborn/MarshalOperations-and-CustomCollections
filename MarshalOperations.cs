namespace VCE.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;


    /// <summary>
    /// This is a workaround for CollectionsMarshal. An unsafe class so it is hidden by default
    /// <para>Is not supported for version .NET 5.0+</para>
    /// </summary>
    //If the .NET version is 5.0 or greater, there is really no reason to use ConvertToSpan for there is a method way better, but it is still available

    public static class MarshalOperations
    {
        /// <summary>
        /// It is an alt version of the CollectionMarshal.AsSpan but is created for any software like Unity that are not using .NET 5 or more.
        /// <para>
        /// The original code can be found in in the CollectionMarshal.AsSpan() in the .NET 7 framework
        ///  However, there are some differences due to the fact this is written under the Unity API
        ///  If you are using .Net 5.0+, this method will not be supported</para>
        /// <remarks>
        /// Remember this is only used for a cover for CollectionMarshal.AsSpan. Once you update your project to 5.0, this will be unavailable
        /// </remarks>
        /// </summary>
        /// <typeparam name="T">any unmanaged arbitrary data</typeparam>
        /// <param name="list">The value that will be wrapped by memory</param>


       public static Span<T> ConvertToSpan<T>(Arr<T> list) where T : unmanaged
            => list is null ? default : new Span<T>(list.items, 0, list.size);

        /// <summary>
        /// If you simply do not want to use Arr, you can still convert the list into span. However, there will be some overhead due to the fact the list internal array 
        /// is private, and "may" need manual copying. So use this sparingly
        /// </summary>
        /// <remarks>
        /// Remember this is only used for a cover for CollectionMarshal.AsSpan. Once you update your project to 5.0, this will be unavailable
        /// </remarks>
        /// <typeparam name="T">unmanaged types</typeparam>
        /// <param name="list">The array of that list</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Span<T> ConvertToSpan<T>(List<T> list) where T : unmanaged //I constrained it to only work with unmanaged types
        {
            if(list.Count == 0) return Span<T>.Empty;

            Span<T> destination = new T[list.Count];
            fixed(T* destPtr = destination)
            {
                for(int i = 0; i < list.Count; i++)
                    destPtr[i] = list[i];
                
            }
            
            return destination;
        }

        /// <summary>
        /// Same thing but with Span included to the argument to decrease allocation efforts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void ConvertToSpan<T>(List<T> list, Span<T> destination) where T : unmanaged //I constrained it to only work with unmanaged types
        {
            if (list is null)
                throw new NullReferenceException(nameof(list));

            if ((uint)destination.Length != (uint)list.Count)
                throw new ArgumentOutOfRangeException("Both List and Span must be equal lengths");

            fixed(T* destPtr = destination)
            {
                for (int i = 0; i < (uint)list.Count; i++)
                    destPtr[i] = list[i];
            }

        }
        /// <summary>
        /// Get reference in the certain index of the internal array of Arr
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>returns a reference or a pointer</returns>
        public unsafe static T* GetRef<T>(Arr<T> arr, int index) where T : unmanaged
        {
            fixed (T* reference = &arr.items[index])
                return reference;
        }
        /// <summary>
        /// Get reference in the certain index of the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public unsafe static T* GetRef<T>(T[] array, int index) where T : unmanaged
        {
            fixed (T* reference = &array[index])
                return reference;
        }
    }
}
