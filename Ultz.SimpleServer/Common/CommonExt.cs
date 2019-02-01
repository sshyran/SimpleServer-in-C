// CommonExt.cs - Ultz.SimpleServer
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;
#if NETCOREAPP2_1
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Ultz.SimpleServer.Internals.Http2;

#endif

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    ///     Contains a set of commonly used extension methods
    /// </summary>
    public static class CommonExt
    {
        /// <summary>
        ///     Applies a set of valves to a target, if possible
        /// </summary>
        /// <param name="valveList">the set of valves to apply</param>
        /// <param name="target">the target <see cref="IConfigurable" /></param>
        /// <typeparam name="T">the type of the target</typeparam>
        public static void ApplyValves<T>(this IEnumerable<Valve> valveList, T target) where T : IConfigurable
        {
            foreach (var applyValve in valveList)
            foreach (var valve in ValveCache.Valves.Values.Where(x => x is IValve<T>))
                if (applyValve.Key == valve.Id)
                    ((IValve<T>) valve).Execute(target, applyValve.Settings.ToDictionary(x => x.Name, x => x.Value));
        }

        /// <summary>
        ///     Adds a <see cref="IValve" /> to a <see cref="Valve" /> collection
        /// </summary>
        /// <param name="coll">the target <see cref="Valve" /> collection</param>
        /// <param name="valve">the <see cref="IValve" /> to add</param>
        /// <param name="settings">any settings that should be passed to the valve</param>
        public static void Add(this ICollection<Valve> coll, IValve valve, params Setting[] settings)
        {
            coll.Add(
                new Valve {Key = valve.Id, Settings = settings == null ? new List<Setting>() : settings.ToList()});
        }

        /// <summary>
        ///     Adds multiple <see cref="IValve" /> to a <see cref="Valve" /> collection, with no settings
        /// </summary>
        /// <param name="coll">the target collection</param>
        /// <param name="valves">the valves to add</param>
        public static void Add(this ICollection<Valve> coll, params IValve[] valves)
        {
            foreach (var valve in valves)
                coll.Add(valve);
        }

        /// <summary>
        ///     Adds valves to an <see cref="IConfigurable" />
        /// </summary>
        /// <param name="configurable">the target</param>
        /// <param name="valves">the valves to add</param>
        public static void AddValve(this IConfigurable configurable, params Valve[] valves)
        {
            foreach (var valve in valves)
                configurable.Valves.Add(valve);
        }

        /// <summary>
        ///     Adds a valve to an <see cref="IConfigurable" /> with the given <see cref="Setting" />s
        /// </summary>
        /// <param name="configurable">the target <see cref="IConfigurable" /></param>
        /// <param name="valve">the valve to add</param>
        /// <param name="settings">the associated settings</param>
        public static void AddValve(this IConfigurable configurable, IValve valve, params Setting[] settings)
        {
            configurable.Valves.Add(valve, settings);
        }

        /// <summary>
        ///     Adds valves to an <see cref="IConfigurable" />
        /// </summary>
        /// <param name="configurable">the target</param>
        /// <param name="valve">the valves to add</param>
        public static void AddValve(this IConfigurable configurable, params IValve[] valve)
        {
            configurable.Valves.Add(valve);
        }

        /// <summary>
        ///     Searches the methods contained within a given <see cref="object" /> using an
        ///     <see cref="IAttributeHandlerResolver" />, for methods which can be defined as handlers. This method then adds the
        ///     resolved handlers to the given <see cref="MinimalServer" />
        /// </summary>
        /// <param name="server">the server to add resolved handlers to</param>
        /// <param name="obj">the object to search for given handlers</param>
        public static void RegisterHandler(this MinimalServer server, object obj)
        {
            server.Handlers.AddRange(
                server.Protocol?.AttributeHandlerResolver?.GetHandlers(obj) ?? new List<IHandler>());
        }

#if NETCOREAPP2_1

        /// <summary>
        ///     Sets the <see cref="ListenerProvider" /> on the <see cref="MinimalServer" /> to one that provides
        ///     <see cref="SslListener" />s with <see cref="TcpConnectionListener" />s, using the given
        ///     <see cref="SslServerAuthenticationOptions" />.
        /// </summary>
        /// <param name="server">the server</param>
        /// <param name="serverAuthenticationOptions">
        ///     SSL Server Authentication Options to be passed to the
        ///     <see cref="SslListener" /> constructor
        /// </param>
        public static void AddSsl(this MinimalServer server, SslServerAuthenticationOptions serverAuthenticationOptions)
        {
            server.ListenerProvider = endpoint =>
                new SslListener(new TcpConnectionListener(endpoint), serverAuthenticationOptions);
        }

        /// <summary>
        ///     Sets the <see cref="ListenerProvider" /> on the <see cref="MinimalServer" /> to one that provides
        ///     <see cref="SslListener" />s with <see cref="TcpConnectionListener" />s, using the given
        ///     <see cref="X509Certificate2" /> and <see cref="RSA" /> private key.
        /// </summary>
        /// <param name="server">the server</param>
        /// <param name="certificate">the certificate</param>
        /// <param name="key">the private key</param>
        public static void AddSsl(this MinimalServer server, X509Certificate2 certificate, RSA key)
        {
            AddSsl(server, certificate.CopyWithPrivateKey(key));
        }

        /// <summary>
        ///     Sets the <see cref="ListenerProvider" /> on the <see cref="MinimalServer" /> to one that provides
        ///     <see cref="SslListener" />s with <see cref="TcpConnectionListener" />s, using the given
        ///     <see cref="X509Certificate2" /> and <see cref="ECDsa" /> private key.
        /// </summary>
        /// <param name="server">the server</param>
        /// <param name="certificate">the certificate</param>
        /// <param name="key">the private key</param>
        public static void AddSsl(this MinimalServer server, X509Certificate2 certificate, ECDsa key)
        {
            AddSsl(server, certificate.CopyWithPrivateKey(key));
        }

        /// <summary>
        ///     Sets the <see cref="ListenerProvider" /> on the <see cref="MinimalServer" /> to one that provides
        ///     <see cref="SslListener" />s with <see cref="TcpConnectionListener" />s, using the given
        ///     <see cref="X509Certificate2" />. The given certificate must have a private key already set.
        /// </summary>
        /// <param name="server">the server</param>
        /// <param name="certificateWithKey">the certificate with the private key already set</param>
        public static void AddSsl(this MinimalServer server, X509Certificate2 certificateWithKey)
        {
            AddSsl(server,
                server.Protocol is HttpTwo
                    ? new SslServerAuthenticationOptions
                    {
                        ServerCertificate = certificateWithKey, EnabledSslProtocols = SslProtocols.Tls12,
                        ClientCertificateRequired = false, EncryptionPolicy = EncryptionPolicy.RequireEncryption,
                        AllowRenegotiation = false,
                        ApplicationProtocols = new List<SslApplicationProtocol> {SslApplicationProtocol.Http2},
                        CertificateRevocationCheckMode = X509RevocationMode.Online
                    }
                    : new SslServerAuthenticationOptions
                    {
                        ServerCertificate = certificateWithKey, EnabledSslProtocols = SslProtocols.Tls12,
                        ClientCertificateRequired = false, EncryptionPolicy = EncryptionPolicy.RequireEncryption,
                        CertificateRevocationCheckMode = X509RevocationMode.Online
                    });
        }
#endif
    }
}