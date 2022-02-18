// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : ISO.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Device structures decoders.
//
// --[ Description ] ----------------------------------------------------------
//
//     Decodes ISO/ECMA floppy structures.
//
// --[ License ] --------------------------------------------------------------
//
//     This library is free software; you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as
//     published by the Free Software Foundation; either version 2.1 of the
//     License, or (at your option) any later version.
//
//     This library is distributed in the hope that it will be useful, but
//     WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//     Lesser General Public License for more details.
//
//     You should have received a copy of the GNU Lesser General Public
//     License along with this library; if not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2022 Natalia Portillo
// ****************************************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Aaru.Decoders.Floppy
{
    // Information from:
    // National Semiconductor PC87332VLJ datasheet
    // SMsC FDC37C78 datasheet
    // Intel 82078 datasheet
    // Intel 82077AA datasheet
    // Toshiba TC8566AF datasheet
    // Fujitsu MB8876A datasheet
    // ECMA-147
    // ECMA-100

    /// <summary>Methods and structures for ISO floppy decoding (also used by Atari ST and others)</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "MemberCanBeInternal"),
     SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class ISO
    {
        /// <summary>ISO floppy track, also used by Atari ST and others</summary>
        public struct Track
        {
            /// <summary>Start of track, 32 bytes set to 0x4E</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] innerGap;
            /// <summary>Track sectors</summary>
            public Sector[] sectors;
            /// <summary>Undefined size</summary>
            public byte[] gap;
        }

        /// <summary>Raw demodulated format for IBM System 34 floppies</summary>
        public struct Sector
        {
            /// <summary>Sector address mark</summary>
            public AddressMark addressMark;
            /// <summary>22 bytes set to 0x4E, set to 0x22 on Commodore 1581</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public byte[] innerGap;
            /// <summary>Sector data block</summary>
            public DataBlock dataBlock;
            /// <summary>Variable bytes set to 0x4E, ECMA defines 54</summary>
            public byte[] outerGap;
        }

        /// <summary>Sector address mark for IBM System 34 floppies, contains sync word</summary>
        public struct AddressMark
        {
            /// <summary>12 bytes set to 0</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] zero;
            /// <summary>3 bytes set to 0xA1</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] aone;
            /// <summary>Set to <see cref="IBMIdType.AddressMark" /></summary>
            public IBMIdType type;
            /// <summary>Track number</summary>
            public byte track;
            /// <summary>Side number</summary>
            public byte side;
            /// <summary>Sector number</summary>
            public byte sector;
            /// <summary>
            ///     <see cref="IBMSectorSizeCode" />
            /// </summary>
            public IBMSectorSizeCode sectorSize;
            /// <summary>CRC16 from <see cref="aone" /> to end of <see cref="sectorSize" /></summary>
            public ushort crc;
        }

        /// <summary>Sector data block for IBM System 34 floppies</summary>
        public struct DataBlock
        {
            /// <summary>12 bytes set to 0</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] zero;
            /// <summary>3 bytes set to 0xA1</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] aone;
            /// <summary>Set to <see cref="IBMIdType.DataMark" /> or to <see cref="IBMIdType.DeletedDataMark" /></summary>
            public IBMIdType type;
            /// <summary>User data</summary>
            public byte[] data;
            /// <summary>CRC16 from <see cref="aone" /> to end of <see cref="data" /></summary>
            public ushort crc;
        }
    }
}