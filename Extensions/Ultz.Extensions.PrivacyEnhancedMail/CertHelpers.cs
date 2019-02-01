// CertHelpers.cs - Ultz.Extensions.PrivacyEnhancedMail
// 
// Copyright (C) 2019 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;

namespace Ultz.Extensions.PrivacyEnhancedMail
{

        public class CertHelpers
        {
            /// <summary>
            ///     This helper function parses an integer size from the reader using the ASN.1 format
            /// </summary>
            /// <param name="rd"></param>
            /// <returns></returns>
            public static int DecodeIntegerSize(BinaryReader rd)
            {
                byte byteValue;
                int count;

                byteValue = rd.ReadByte();
                if (byteValue != 0x02) // indicates an ASN.1 integer value follows
                    return 0;

                byteValue = rd.ReadByte();
                if (byteValue == 0x81)
                {
                    count = rd.ReadByte(); // data size is the following byte
                }
                else if (byteValue == 0x82)
                {
                    var hi = rd.ReadByte(); // data size in next 2 bytes
                    var lo = rd.ReadByte();
                    count = BitConverter.ToUInt16(new[] {lo, hi}, 0);
                }
                else
                {
                    count = byteValue; // we already have the data size
                }

                //remove high order zeros in data
                while (rd.ReadByte() == 0x00) count -= 1;

                rd.BaseStream.Seek(-1, SeekOrigin.Current);

                return count;
            }

            /// <summary>
            /// </summary>
            /// <param name="inputBytes"></param>
            /// <param name="alignSize"></param>
            /// <returns></returns>
            public static byte[] AlignBytes(byte[] inputBytes, int alignSize)
            {
                var inputBytesSize = inputBytes.Length;

                if (alignSize != -1 && inputBytesSize < alignSize)
                {
                    var buf = new byte[alignSize];
                    for (var i = 0; i < inputBytesSize; ++i) buf[i + (alignSize - inputBytesSize)] = inputBytes[i];

                    return buf;
                }

                return inputBytes; // Already aligned, or doesn't need alignment
            }
        }
}
