// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License MIT. See License.md in the repository root for more information.

// Ported from um\dcommon.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

namespace TerraFX.Interop
{
    public /* blittable */ struct D2D1_POINT_2U
    {
        #region Fields
        internal D2D_POINT_2U _value;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="D2D1_POINT_2U" /> struct.</summary>
        /// <param name="value">The <see cref="D2D_POINT_2U" /> used to initialize the instance.</param>
        public D2D1_POINT_2U(D2D_POINT_2U value)
        {
            _value = value;
        }
        #endregion

        #region Operators
        /// <summary>Implicitly converts a <see cref="D2D1_POINT_2U" /> value to a <see cref="D2D_POINT_2U" /> value.</summary>
        /// <param name="value">The <see cref="D2D1_POINT_2U" /> value to convert.</param>
        public static implicit operator D2D_POINT_2U(D2D1_POINT_2U value)
        {
            return value._value;
        }

        /// <summary>Implicitly converts a <see cref="D2D_POINT_2U" /> value to a <see cref="D2D1_POINT_2U" /> value.</summary>
        /// <param name="value">The <see cref="D2D_POINT_2U" /> value to convert.</param>
        public static implicit operator D2D1_POINT_2U(D2D_POINT_2U value)
        {
            return new D2D1_POINT_2U(value);
        }
        #endregion
    }
}
