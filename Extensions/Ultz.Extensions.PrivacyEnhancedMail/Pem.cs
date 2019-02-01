// Pem.cs - Ultz.SimpleServer
// 
// Copyright (C) 2018 Ultz Limited
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

#region

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace Ultz.Extensions.PrivacyEnhancedMail
{
    /// <summary>
    ///     A class containing PEM format helper methods
    /// </summary>
    public static class Pem
    {
        /// <summary>
        ///     Gets a <see cref="X509Certificate2" /> from a Base64-encoded certificate.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string cert)
        {
            return new X509Certificate2(PemExt.GetBytesFromPem(cert, "CERTIFICATE"));
        }

#if NET472 || NETCOREAPP2_0 || NETSTANDARD2_1
        /// <summary>
        ///     Gets a <see cref="X509Certificate2" /> from a Base64-encoded certificate and private key.
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string cert, string key)
        {
            return GetCertificate(cert).CopyWithPrivateKey(
                PemExt.DecodeRsaPrivateKey(PemExt.GetBytesFromPem(key,
                    "RSA PRIVATE KEY")));
        }
#endif

        public static (X509Certificate2, RSA) GetCertificateAndKey(string cert, string key)
        {
            return (GetCertificate(cert), PemExt.DecodeRsaPrivateKey(PemExt.GetBytesFromPem(key, "RSA PRIVATE KEY")));
        }
    }
}
