// SslConnectorValve.cs - Ultz.SimpleServer
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

#if NETCOREAPP2_1

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Ultz.SimpleServer.Internals.Http2;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    /// A <see cref="IValve"/> that's bound to a <see cref="Connector"/>, which will wrap the <see cref="Connector.Listener"/> with a <see cref="SslListener"/>
    /// </summary>
    public class SslConnectorValve : IValve<Connector>
    {
        /// <inheritdoc />
        public void Execute(Connector obj, Dictionary<string, string> settings)
        {
            // 2 PEM settings:
            //
            // pem(Cert/Key):     this is for non-daemon applications so that
            //                    the user can embed the certificate/key content in
            //                    their code and pass it without storing as a file.
            //
            // pem(Cert/Key)Path: For daemon applications, the certificate/key can
            //                    be stored as a file.
            //
            // pem(Cert/Key) should be used if both properties are present. However,
            // in an ideal world, the user does not provide both.
            X509Certificate2 cert;
            if (settings.ContainsKey("pemCert"))
                cert = new X509Certificate2(CommonExt.GetBytesFromPem(settings["pemCert"], "CERTIFICATE"));
            else if (settings.ContainsKey("pemCertPath"))
                cert = new X509Certificate2(CommonExt.GetBytesFromPem(File.ReadAllText(settings["pemCert"]),
                    "CERTIFICATE"));
            else
                throw new ArgumentException("The settings given contains neither a pemCert or pemCertPath property.",
                    nameof(settings));
            if (settings.ContainsKey("pemKey"))
                cert = cert.CopyWithPrivateKey(
                    CommonExt.DecodeRsaPrivateKey(CommonExt.GetBytesFromPem(settings["pemKey"], "RSA PRIVATE KEY")));
            else if (settings.ContainsKey("pemKeyPath"))
                cert = cert.CopyWithPrivateKey(
                    CommonExt.DecodeRsaPrivateKey(CommonExt.GetBytesFromPem(File.ReadAllText(settings["pemKeyPath"]),
                        "RSA PRIVATE KEY")));
            else
                throw new ArgumentException("The settings given contains neither a pemKey or pemKeyPath property.",
                    nameof(settings));
            obj.Listener = new SslListener(obj.Listener, new SslServerAuthenticationOptions
            {
                ServerCertificate = cert,
                AllowRenegotiation = settings.ContainsKey("alpnAllowRenegotiation") &&
                                     bool.Parse(settings["alpnAllowRenegotiaion"]),
                ApplicationProtocols = obj.Service.Protocol is HttpTwo
                    ? new List<SslApplicationProtocol> {SslApplicationProtocol.Http2, SslApplicationProtocol.Http11}
                    : settings.ContainsKey("alpnProtocols")
                        ? settings["alpnProtocols"].Split(';')
                            .Select(protocol => new SslApplicationProtocol(protocol)).ToList()
                        : null,
                CertificateRevocationCheckMode = settings.ContainsKey("alpnRevokeCheck")
                    ? settings["alpnRevokeCheck"].ToLower() == "none" ? X509RevocationMode.NoCheck :
                    settings["alpnRevokeCheck"].ToLower() == "offline" ? X509RevocationMode.Offline :
                    X509RevocationMode.Online
                    : X509RevocationMode.Online,
                ClientCertificateRequired = settings.ContainsKey("alpnClientCertNeeded") &&
                                            bool.Parse(settings["alpnClientCertNeeded"]),
                EnabledSslProtocols = !settings.ContainsKey("protocol") ? SslProtocols.None :
                    settings["protocol"].ToLower() == "tls1" ? SslProtocols.Tls :
                    settings["protocol"].ToLower() == "tls11" ? SslProtocols.Tls11 :
                    settings["protocol"].ToLower() == "tls12" ? SslProtocols.Tls12 : SslProtocols.None,
                EncryptionPolicy = !settings.ContainsKey("encryption") ? EncryptionPolicy.RequireEncryption :
                    settings["encryption"].ToLower() == "allowNone" ? EncryptionPolicy.AllowNoEncryption :
                    settings["encryption"].ToLower() == "none" ? EncryptionPolicy.NoEncryption :
                    EncryptionPolicy.RequireEncryption
            });
        }

        /// <inheritdoc />
        public string Id => "simpleserver.ssl";
    }
}
#endif