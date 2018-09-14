// SslListener.cs - Ultz.SimpleServer
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

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    /// A wrapper that applies an <see cref="SslStream"/> to accepted <see cref="IConnection"/>s
    /// </summary>
    public class SslListener : IListener
    {
        private readonly IListener _child;
        private readonly SslServerAuthenticationOptions _opts;

        /// <summary>
        /// Creates an instance of <see cref="SslListener"/>, wrapping the given <see cref="IListener"/> with the given <see cref="SslServerAuthenticationOptions"/>
        /// </summary>
        /// <param name="listener">the base listener to wrap</param>
        /// <param name="sslServerAuthenticationOptions">AuthenticateAsServerAsync options</param>
        public SslListener(IListener listener, SslServerAuthenticationOptions sslServerAuthenticationOptions)
        {
            _child = listener;
            _opts = sslServerAuthenticationOptions;
            _opts.ServerCertificate = new X509Certificate2(_opts.ServerCertificate.Export(X509ContentType.Pkcs12));
            // the above line is a workaround for SS-19
        }


        /// <inheritdoc />
        public void Dispose()
        {
            _child.Dispose();
        }

        /// <inheritdoc />
        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task<IConnection> AcceptAsync()
        {
            return new SecureConnection(await _child.AcceptAsync(), _opts);
        }

        /// <inheritdoc />
        public void Start()
        {
            _child.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            _child.Stop();
        }

        /// <inheritdoc />
        public void RunChecks()
        {
            if (_opts == null)
                throw new NullReferenceException("No options passed to the SslListener.");
            if (_opts.ServerCertificate == null)
                throw new NullReferenceException("ServerCertificate is null. Cannot continue.");
            _child.RunChecks();
        }

        /// <inheritdoc />
        public bool Active => _child.Active;

        internal class SecureConnection : IConnection
        {
            private readonly IConnection _child;

            public SecureConnection(IConnection child, SslServerAuthenticationOptions opts)
            {
                _child = child;
                Stream = new SslStream(child.Stream);
                ((SslStream) Stream).AuthenticateAsServerAsync(opts, CancellationToken.None).GetAwaiter()
                    .GetResult();
            }

            public Stream Stream { get; }
            public bool Connected => _child.Connected;

            public void Close()
            {
                Stream.Dispose();
            }

            public EndPoint LocalEndPoint => _child.LocalEndPoint;
            public EndPoint RemoteEndPoint => _child.RemoteEndPoint;
            public int Id => _child.Id;
        }
    }
}
#endif