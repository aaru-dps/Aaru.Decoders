// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : DDS.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Device structures decoders.
//
// --[ Description ] ----------------------------------------------------------
//
//     Decodes Blu-ray Disc Definition Structure.
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
// Copyright © 2011-2023 Natalia Portillo
// ****************************************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Aaru.Console;
using Aaru.Helpers;

namespace Aaru.Decoders.Bluray
{
    /// Information from the following standards:
    /// ANSI X3.304-1997
    /// T10/1048-D revision 9.0
    /// T10/1048-D revision 10a
    /// T10/1228-D revision 7.0c
    /// T10/1228-D revision 11a
    /// T10/1363-D revision 10g
    /// T10/1545-D revision 1d
    /// T10/1545-D revision 5
    /// T10/1545-D revision 5a
    /// T10/1675-D revision 2c
    /// T10/1675-D revision 4
    /// T10/1836-D revision 2g
    [SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "MemberCanBeInternal"),
     SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "NotAccessedField.Global")]
    public static class DDS
    {
        #region Private constants
        /// <summary>Disc Definition Structure Identifier "DS"</summary>
        const ushort DDSIdentifier = 0x4453;
        #endregion Private constants

        #region Public structures
        public struct DiscDefinitionStructure
        {
            /// <summary>Bytes 0 to 1 Data Length</summary>
            public ushort DataLength;
            /// <summary>Byte 2 Reserved</summary>
            public byte Reserved1;
            /// <summary>Byte 3 Reserved</summary>
            public byte Reserved2;
            /// <summary>Bytes 4 to 5 "DS"</summary>
            public ushort Signature;
            /// <summary>Byte 6 DDS format</summary>
            public byte Format;
            /// <summary>Byte 7 Reserved</summary>
            public byte Reserved3;
            /// <summary>Bytes 8 to 11 DDS update count</summary>
            public uint UpdateCount;
            /// <summary>Bytes 12 to 19 Reserved</summary>
            public ulong Reserved4;
            /// <summary>Bytes 20 to 23 First PSN of Drive Area</summary>
            public uint DriveAreaPSN;
            /// <summary>Bytes 24 to 27 Reserved</summary>
            public uint Reserved5;
            /// <summary>Bytes 28 to 31 First PSN of Defect List</summary>
            public uint DefectListPSN;
            /// <summary>Bytes 32 to 35 Reserved</summary>
            public uint Reserved6;
            /// <summary>Bytes 36 to 39 PSN of LSN 0 of user data area</summary>
            public uint PSNofLSNZero;
            /// <summary>Bytes 40 to 43 Last LSN of user data area</summary>
            public uint LastUserAreaLSN;
            /// <summary>Bytes 44 to 47 ISA0 size</summary>
            public uint ISA0;
            /// <summary>Bytes 48 to 51 OSA size</summary>
            public uint OSA;
            /// <summary>Bytes 52 to 55 ISA1 size</summary>
            public uint ISA1;
            /// <summary>Byte 56 Spare Area full flags</summary>
            public byte SpareAreaFullFlags;
            /// <summary>Byte 57 Reserved</summary>
            public byte Reserved7;
            /// <summary>Byte 58 Disc type specific field</summary>
            public byte DiscTypeSpecificField1;
            /// <summary>Byte 59 Reserved</summary>
            public byte Reserved8;
            /// <summary>Byte 60 to 63 Disc type specific field</summary>
            public uint DiscTypeSpecificField2;
            /// <summary>Byte 64 to 67 Reserved</summary>
            public uint Reserved9;
            /// <summary>Bytes 68 to 99 Status bits of INFO1/2 and PAC1/2 on L0 and L1</summary>
            public byte[] StatusBits;
            /// <summary>Bytes 100 to end Disc type specific data</summary>
            public byte[] DiscTypeSpecificData;
        }
        #endregion Public structures

        #region Public methods
        public static DiscDefinitionStructure? Decode(byte[] DDSResponse)
        {
            if(DDSResponse == null)
                return null;

            var decoded = new DiscDefinitionStructure
            {
                DataLength = BigEndianBitConverter.ToUInt16(DDSResponse, 0),
                Reserved1  = DDSResponse[2],
                Reserved2  = DDSResponse[3],
                Signature  = BigEndianBitConverter.ToUInt16(DDSResponse, 4)
            };

            if(decoded.Signature != DDSIdentifier)
            {
                AaruConsole.DebugWriteLine("BD DDS decoder", "Found incorrect DDS signature (0x{0:X4})",
                                           decoded.Signature);

                return null;
            }

            decoded.Format                 = DDSResponse[6];
            decoded.Reserved3              = DDSResponse[7];
            decoded.UpdateCount            = BigEndianBitConverter.ToUInt32(DDSResponse, 8);
            decoded.Reserved4              = BigEndianBitConverter.ToUInt64(DDSResponse, 12);
            decoded.DriveAreaPSN           = BigEndianBitConverter.ToUInt32(DDSResponse, 20);
            decoded.Reserved5              = BigEndianBitConverter.ToUInt32(DDSResponse, 24);
            decoded.DefectListPSN          = BigEndianBitConverter.ToUInt32(DDSResponse, 28);
            decoded.Reserved6              = BigEndianBitConverter.ToUInt32(DDSResponse, 32);
            decoded.PSNofLSNZero           = BigEndianBitConverter.ToUInt32(DDSResponse, 36);
            decoded.LastUserAreaLSN        = BigEndianBitConverter.ToUInt32(DDSResponse, 40);
            decoded.ISA0                   = BigEndianBitConverter.ToUInt32(DDSResponse, 44);
            decoded.OSA                    = BigEndianBitConverter.ToUInt32(DDSResponse, 48);
            decoded.ISA1                   = BigEndianBitConverter.ToUInt32(DDSResponse, 52);
            decoded.SpareAreaFullFlags     = DDSResponse[56];
            decoded.Reserved7              = DDSResponse[57];
            decoded.DiscTypeSpecificField1 = DDSResponse[58];
            decoded.Reserved8              = DDSResponse[59];
            decoded.DiscTypeSpecificField2 = BigEndianBitConverter.ToUInt32(DDSResponse, 60);
            decoded.Reserved9              = BigEndianBitConverter.ToUInt32(DDSResponse, 64);
            decoded.StatusBits             = new byte[32];
            Array.Copy(DDSResponse, 68, decoded.StatusBits, 0, 32);
            decoded.DiscTypeSpecificData = new byte[DDSResponse.Length - 100];
            Array.Copy(DDSResponse, 100, decoded.DiscTypeSpecificData, 0, DDSResponse.Length - 100);

            return decoded;
        }

        public static string Prettify(DiscDefinitionStructure? DDSResponse)
        {
            if(DDSResponse == null)
                return null;

            DiscDefinitionStructure response = DDSResponse.Value;

            var sb = new StringBuilder();

            sb.AppendFormat("DDS Format: 0x{0:X2}", response.Format).AppendLine();
            sb.AppendFormat("DDS has ben updated {0} times", response.UpdateCount).AppendLine();
            sb.AppendFormat("First PSN of Drive Area: 0x{0:X8}", response.DriveAreaPSN).AppendLine();
            sb.AppendFormat("First PSN of Defect List: 0x{0:X8}", response.DefectListPSN).AppendLine();
            sb.AppendFormat("PSN of User Data Area's LSN 0: 0x{0:X8}", response.PSNofLSNZero).AppendLine();
            sb.AppendFormat("Last User Data Area's LSN 0: 0x{0:X8}", response.LastUserAreaLSN).AppendLine();
            sb.AppendFormat("ISA0 size: {0}", response.ISA0).AppendLine();
            sb.AppendFormat("OSA size: {0}", response.OSA).AppendLine();
            sb.AppendFormat("ISA1 size: {0}", response.ISA1).AppendLine();
            sb.AppendFormat("Spare Area Full Flags: 0x{0:X2}", response.SpareAreaFullFlags).AppendLine();
            sb.AppendFormat("Disc Type Specific Field 1: 0x{0:X2}", response.DiscTypeSpecificField1).AppendLine();
            sb.AppendFormat("Disc Type Specific Field 2: 0x{0:X8}", response.DiscTypeSpecificField2).AppendLine();
            sb.AppendFormat("Blu-ray DDS Status Bits in hex follows:");
            sb.AppendLine(PrintHex.ByteArrayToHexArrayString(response.StatusBits, 80));
            sb.AppendFormat("Blu-ray DDS Disc Type Specific Data in hex follows:");
            sb.AppendLine(PrintHex.ByteArrayToHexArrayString(response.DiscTypeSpecificData, 80));

        #if DEBUG
            if(response.Reserved1 != 0)
                sb.AppendFormat("Reserved1 = 0x{0:X2}", response.Reserved1).AppendLine();

            if(response.Reserved2 != 0)
                sb.AppendFormat("Reserved2 = 0x{0:X2}", response.Reserved2).AppendLine();

            if(response.Reserved3 != 0)
                sb.AppendFormat("Reserved3 = 0x{0:X2}", response.Reserved3).AppendLine();

            if(response.Reserved4 != 0)
                sb.AppendFormat("Reserved4 = 0x{0:X16}", response.Reserved4).AppendLine();

            if(response.Reserved5 != 0)
                sb.AppendFormat("Reserved5 = 0x{0:X8}", response.Reserved5).AppendLine();

            if(response.Reserved6 != 0)
                sb.AppendFormat("Reserved6 = 0x{0:X8}", response.Reserved6).AppendLine();

            if(response.Reserved7 != 0)
                sb.AppendFormat("Reserved7 = 0x{0:X2}", response.Reserved7).AppendLine();

            if(response.Reserved8 != 0)
                sb.AppendFormat("Reserved8 = 0x{0:X2}", response.Reserved8).AppendLine();

            if(response.Reserved9 != 0)
                sb.AppendFormat("Reserved9 = 0x{0:X8}", response.Reserved9).AppendLine();
        #endif

            return sb.ToString();
        }

        public static string Prettify(byte[] DDSResponse) => Prettify(Decode(DDSResponse));
        #endregion Public methods
    }
}