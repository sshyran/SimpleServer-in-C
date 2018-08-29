#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Common
{
    public class SimpleServer : IDisposable, ICollection<Service>
    {
        private ILogger _logger;
        private ILoggerProvider _loggerProvider;
        private bool _disposed;

        public SimpleServer(ILoggerProvider logger = null)
        {
            _loggerProvider = logger;
            _logger = logger?.CreateLogger("SimpleServer");
        }

        public IReadOnlyDictionary<string, Service> Services { get; } = new Dictionary<string, Service>();

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
                    _logger.LogError(new EventId(working,"StartFail"), ex, service.Key+" failed to start");
                }
            }

            _logger.LogInformation(working + "/" + Services.Count + " services started");
        }

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

        public void Dispose()
        {
            Stop(true);
        }

        public IEnumerator<Service> GetEnumerator()
        {
            return Services.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Service item)
        {
            var services = (Dictionary<string, Service>) Services;
            var name = item.GetType().Name;
            var number = 1;
            while (services.ContainsKey(name + number))
            {
                number++;
            }

            services.Add(name + number, item);
        }

        public void Clear()
        {
            ((IDictionary) Services).Clear();
        }

        public bool Contains(Service item)
        {
            return Services.Values.Contains(item);
        }

        public void CopyTo(Service[] array, int arrayIndex)
        {
            Array.Copy(Services.Values.ToArray(),0,array,arrayIndex, array.Length);
        }

        public bool Remove(Service item)
        {
            return ((IDictionary<string, string>) Services).Remove(Services.First(x => x.Value == item).Key);
        }

        public int Count => Services.Count;
        public bool IsReadOnly => false;
    }
}