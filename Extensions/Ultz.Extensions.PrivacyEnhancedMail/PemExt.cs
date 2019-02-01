// PemExt.cs - Ultz.Extensions.PrivacyEnhancedMail
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
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Ultz.Extensions.PrivacyEnhancedMail
{
    public static class PemExt
    {
        /// <summary>
        ///     Removes borders from a PEM certificate file, and decodes the Base64 data.
        /// </summary>
        /// <param name="pemString">the PEM file</param>
        /// <param name="section">the PEM border name</param>
        /// <returns></returns>
        public static byte[] GetBytesFromPem(string pemString, string section)
        {
            var header = string.Format("-----BEGIN {0}-----", section);
            var footer = string.Format("-----END {0}-----", section);

            var start = pemString.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

            return end < 0 ? null : Convert.FromBase64String(pemString.Substring(start, end));
        }

        /// <summary>
        ///     Decodes an <see cref="RSA" /> private key from its <see cref="byte" /> form.
        /// </summary>
        /// <param name="privateKeyBytes">the raw private key</param>
        /// <returns>the decoded private key</returns>
        public static RSA DecodeRsaPrivateKey(byte[] privateKeyBytes)
        {
            var ms = new MemoryStream(privateKeyBytes);
            var rd = new BinaryReader(ms);

            try
            {
                byte byteValue;
                ushort shortValue;

                shortValue = rd.ReadUInt16();

                switch (shortValue)
                {
                    case 0x8130:
                        // If true, data is little endian since the proper logical seq is 0x30 0x81
                        rd.ReadByte(); //advance 1 byte
                        break;
                    case 0x8230:
                        rd.ReadInt16(); //advance 2 bytes
                        break;
                    default:
                        Debug.Assert(false); // Improper ASN.1 format
                        return null;
                }

                shortValue = rd.ReadUInt16();
                if (shortValue != 0x0102) // (version number)
                {
                    Debug.Assert(false); // Improper ASN.1 format, unexpected version number
                    return null;
                }

                byteValue = rd.ReadByte();
                if (byteValue != 0x00)
                {
                    Debug.Assert(false); // Improper ASN.1 format
                    return null;
                }

                // The data following the version will be the ASN.1 data itself, which in our case
                // are a sequence of integers.

                // In order to solve a problem with instancing RSACryptoServiceProvider
                // via default constructor on .net 4.0 this is a hack
//                var parms = new CspParameters();
//                parms.Flags = CspProviderFlags.NoFlags;
//                parms.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
//                parms.ProviderType =
//                    Environment.OSVersion.Version.Major > 5 ||
//                    Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1
//                        ? 0x18
//                        : 1;
//
//                var rsa = new RSACryptoServiceProvider(parms);
                var rsAparams = new RSAParameters();

                rsAparams.Modulus = rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd));

                // Argh, this is a pain.  From emperical testing it appears to be that RSAParameters doesn't like byte buffers that
                // have their leading zeros removed.  The RFC doesn't address this area that I can see, so it's hard to say that this
                // is a issue, but it sure would be helpful if it allowed that. So, there's some extra code here that knows what the
                // sizes of the various components are supposed to be.  Using these sizes we can ensure the buffer sizes are exactly
                // what the RSAParameters expect.  Thanks, Microsoft.
                var traits = new RsaParameterTraits(rsAparams.Modulus.Length * 8);

                rsAparams.Modulus = CertHelpers.AlignBytes(rsAparams.Modulus, traits.SizeMod);
                rsAparams.Exponent =
                    CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeExp);
                rsAparams.D = CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeD);
                rsAparams.P = CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeP);
                rsAparams.Q = CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeQ);
                rsAparams.DP = CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeDp);
                rsAparams.DQ = CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeDq);
                rsAparams.InverseQ =
                    CertHelpers.AlignBytes(rd.ReadBytes(CertHelpers.DecodeIntegerSize(rd)), traits.SizeInvQ);

                //rsa.ImportParameters(rsAparams);
                var rsa = RSA.Create();
                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception)
            {
                Debug.Assert(false);
                return null;
            }
            finally
            {
                rd.Close();
            }
        }
    }
}
