#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

#endregion

namespace Ultz.SimpleServer.Common
{
    public static class CommonExt
    {
        public static void ApplyValves<T>(this IEnumerable<Valve> valveList, T target) where T : IConfigurable
        {
            foreach (var applyValve in valveList)
            foreach (var valve in ValveCache.Valves.Values.Where(x => x is IValve<T>))
                if (applyValve.Key == valve.Id)
                    ((IValve<T>) valve).Execute(target, applyValve.Settings.ToDictionary(x => x.Name, x => x.Value));
        }

        public static void Add(this ICollection<Valve> coll, IValve valve, params Setting[] settings)
        {
            coll.Add(
                new Valve() {Key = valve.Id, Settings = settings == null ? new List<Setting>() : settings.ToList()});
        }

        public static void Add(this ICollection<Valve> coll, params IValve[] valves)
        {
            foreach (var valve in valves)
                coll.Add(valve);
        }

        public static void AddValve(this IConfigurable configurable, params Valve[] valves)
        {
            foreach (var valve in valves)
                configurable.Valves.Add(valve);
        }

        public static void AddValve(this IConfigurable configurable, IValve valve, params Setting[] settings)
        {
            configurable.Valves.Add(valve, settings);
        }

        public static void AddValve(this IConfigurable configurable, params IValve[] valve)
        {
            configurable.Valves.Add(valve);
        }

        // PEM helpers credit: https://www.codeproject.com/Articles/162194/Certificates-to-DB-and-Back

        #region PEM Helpers

        private class RsaParameterTraits
        {
            public readonly int SizeD = -1;
            public readonly int SizeDp = -1;
            public readonly int SizeDq = -1;
            public readonly int SizeExp = -1;
            public readonly int SizeInvQ = -1;

            public readonly int SizeMod = -1;
            public readonly int SizeP = -1;
            public readonly int SizeQ = -1;

            public RsaParameterTraits(int modulusLengthInBits)
            {
                // The modulus length is supposed to be one of the common lengths, which is the commonly referred to strength of the key,
                // like 1024 bit, 2048 bit, etc.  It might be a few bits off though, since if the modulus has leading zeros it could show
                // up as 1016 bits or something like that.
                int assumedLength;
                var logbase = Math.Log(modulusLengthInBits, 2);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (logbase == (int) logbase)
                {
                    // It's already an even power of 2
                    assumedLength = modulusLengthInBits;
                }
                else
                {
                    // It's not an even power of 2, so round it up to the nearest power of 2.
                    assumedLength = (int) (logbase + 1.0);
                    assumedLength = (int) Math.Pow(2, assumedLength);
                    Debug.Assert(false); // Can this really happen in the field?  I've never seen it, so if it happens
                    // you should verify that this really does the 'right' thing!
                }

                switch (assumedLength)
                {
                    case 1024:
                        SizeMod = 0x80;
                        SizeExp = -1;
                        SizeD = 0x80;
                        SizeP = 0x40;
                        SizeQ = 0x40;
                        SizeDp = 0x40;
                        SizeDq = 0x40;
                        SizeInvQ = 0x40;
                        break;
                    case 2048:
                        SizeMod = 0x100;
                        SizeExp = -1;
                        SizeD = 0x100;
                        SizeP = 0x80;
                        SizeQ = 0x80;
                        SizeDp = 0x80;
                        SizeDq = 0x80;
                        SizeInvQ = 0x80;
                        break;
                    case 4096:
                        SizeMod = 0x200;
                        SizeExp = -1;
                        SizeD = 0x200;
                        SizeP = 0x100;
                        SizeQ = 0x100;
                        SizeDp = 0x100;
                        SizeDq = 0x100;
                        SizeInvQ = 0x100;
                        break;
                    default:
                        Debug.Assert(false); // Unknown key size?
                        break;
                }
            }
        }

        private class CertHelpers
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

        #endregion
    }
}