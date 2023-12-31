// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using TerraFX.Collections;

namespace TerraFX;

public partial struct UnmanagedArray<T, TData>
{
    /// <summary>An enumerator which can iterate through the items in an array.</summary>
    public struct Enumerator : IRefEnumerator<T>
    {
        private readonly UnmanagedArray<T, TData> _array;
        private nuint _index;

        internal Enumerator(UnmanagedArray<T, TData> array)
        {
            _array = array;
            _index = nuint.MaxValue;
        }

        /// <inheritdoc />
        public readonly T Current => CurrentRef;

        /// <inheritdoc />
        public readonly ref readonly T CurrentRef => ref _array.GetReferenceUnsafe(_index);

        /// <inheritdoc />
        public bool MoveNext()
        {
            var succeeded = true;
            var index = unchecked(_index + 1);

            if (index == _array.Length)
            {
                index--;
                succeeded = false;
            }

            _index = index;
            return succeeded;            
        }

        /// <inheritdoc />
        public void Reset() => _index = nuint.MaxValue;

        readonly object IEnumerator.Current => Current;

        readonly void IDisposable.Dispose() { }
    }
}
