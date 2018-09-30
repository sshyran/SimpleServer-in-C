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

#if NETCOREAPP2_1 || NETSTANDARD2_1

#region

using System.Security.Cryptography.X509Certificates;

#endregion

namespace Ultz.SimpleServer.Common
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
            return new X509Certificate2(CommonExt.GetBytesFromPem(cert, "CERTIFICATE"));
        }

        /// <summary>
        ///     Gets a <see cref="X509Certificate2" /> from a Base64-encoded certificate and private key.
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(string cert, string key)
        {
            return GetCertificate(cert).CopyWithPrivateKey(
                CommonExt.DecodeRsaPrivateKey(CommonExt.GetBytesFromPem(key,
                    "RSA PRIVATE KEY")));
        }
    }
}
#endif