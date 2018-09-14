// SimpleServer.cs - Ultz.SimpleServer
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    /// A server that will start and stop multiple <see cref="Service"/>s in union
    /// </summary>
    public class SimpleServer : IDisposable, ICollection<Service>
    {
        private bool _disposed;
        private readonly ILogger _logger;
        private readonly ILoggerProvider _loggerProvider;

        /// <summary>
        /// Creates an instance of <see cref="SimpleServer"/>, optionally with a logger
        /// </summary>
        /// <param name="logger"></param>
        public SimpleServer(ILoggerProvider logger = null)
        {
            _loggerProvider = logger;
            _logger = logger?.CreateLogger("SimpleServer");
        }

        /// <summary>
        /// A read only dictionary of <see cref="Service"/>s, indexed by their names
        /// </summary>
        public IReadOnlyDictionary<string, Service> Services { get; } = new Dictionary<string, Service>();

        /// <inheritdoc />
        public IEnumerator<Service> GetEnumerator()
        {
            return Services.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a service to <see cref="Services"/>
        /// </summary>
        /// <param name="item">the service to add</param>
        public void Add(Service item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            var services = (Dictionary<string, Service>) Services;
            var name = item.GetType().Name;
            var number = 1;
            while (services.ContainsKey(name + number)) number++;

            services.Add(name + number, item);
        }
        
        /// <summary>
        /// Adds a service to <see cref="Services"/> with the given name
        /// </summary>
        /// <param name="name">the given name</param>
        /// <param name="item">the service to add</param>
        public void Add(string name,Service item)
        {
            var services = (Dictionary<string, Service>) Services;
            services.Add(name, item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            ((IDictionary) Services).Clear();
        }

        /// <inheritdoc />
        public bool Contains(Service item)
        {
            return Services.Values.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(Service[] array, int arrayIndex)
        {
            Array.Copy(Services.Values.ToArray(), 0, array, arrayIndex, array.Length);
        }

        /// <inheritdoc />
        public bool Remove(Service item)
        {
            // Honestly this error is bullsh*t
            // ReSharper disable once SuspiciousTypeConversion.Global
            return ((IDictionary<string, string>) Services).Remove(Services.First(x => x.Value == item).Key);
        }

        /// <inheritdoc />
        public int Count => Services.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Dispose()
        {
            Stop(true);
        }

        /// <summary>
        /// Starts all of <see cref="Services"/> contained within this <see cref="SimpleServer"/>
        /// </summary>
        /// <exception cref="ObjectDisposedException">thrown if this <see cref="SimpleServer"/> has been <see cref="Dispose"/>d</exception>
        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException("Object has been disposed");
            _logger.LogInformation("SimpleServer is starting");
            _logger.LogTrace(Services.Count + " service(s) awaiting startup");
            var working = 0;
            foreach (var service in Services)
            {
                _logger.LogTrace("Service Name: " + service.Key + "\r\nService Type: " +
                                 service.Value.GetType().FullName);
                try
                {
                    service.Value.LoggerProvider = _loggerProvider;
                    service.Value.Start();
                    working++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, service.Key + " failed to start!");
                    _logger.LogError(new EventId(working, "StartFail"), ex, service.Key + " failed to start");
                }
            }

            _logger.LogInformation(working + "/" + Services.Count + " services started");
        }

        /// <summary>
        /// Stops all of the <see cref="Services"/> contained within this <see cref="SimpleServer"/>, optionally disposing them
        /// </summary>
        /// <param name="dispose">if the contained <see cref="Services"/> should be disposed</param>
        /// <exception cref="ObjectDisposedException">thrown if this <see cref="SimpleServer"/> has been <see cref="Dispose"/>d</exception>
        public void Stop(bool dispose = false)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object has been disposed");
            _logger.LogInformation("SimpleServer is stopping");
            var servicesToStop = Services.Where(x => x.Value.Active).ToList();
            _logger.LogTrace(servicesToStop.Count + " service(s) awaiting shutdown");
            foreach (var service in servicesToStop)
            {
                _logger.LogTrace("Service Name: " + service.Key + "\r\nService Type: " +
                                 service.Value.GetType().FullName);
                try
                {
                    service.Value.Stop();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, service.Key + " failed to stop gracefully!");
                }

                if (dispose)
                    service.Value.Dispose();
            }

            _logger.LogInformation(servicesToStop.Count + " service(s) shutdown");
            if (dispose)
                _logger.LogInformation("SimpleServer has been disposed");
            _disposed = dispose;
        }
    }
}