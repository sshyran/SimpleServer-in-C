using SimpleServer.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Internals
{
    public class SimpleServerResponse
    {
        private SimpleServerConnection client;

        internal SimpleServerResponse(SimpleServerRequest request, SimpleServerConnection client)
        {
            Headers = new Dictionary<string, string>();

            this.client = client;
            Request = request;
            OutputStream = new MemoryStream();

            Version = Request.Version;
            StatusCode = 200;
            ReasonPhrase = "OK";
        }
        

        SimpleServerRequest Request { get; set; }

        /// <summary>
        /// Gets the headers of the HTTP response.
        /// </summary>
        public Dictionary<string,string> Headers { get; private set; }

        /// <summary>
        /// Gets the stream containing the content of this response.
        /// </summary>
        public Stream OutputStream { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP reason phrase.
        /// </summary>
        public string ReasonPhrase { get; set; }

        private async Task Send()
        {
            var outputStream = OutputStream as MemoryStream;
            outputStream.Seek(0, SeekOrigin.Begin);

            var socketStream = client.Stream;
            

            string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                            Headers +
                            $"Content-Length: {outputStream.Length}\r\n" +
                            "\r\n";

            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            await socketStream.WriteAsync(headerArray, 0, headerArray.Length);
            await outputStream.CopyToAsync(socketStream);

            await socketStream.FlushAsync();

        }

        /// <summary>
        /// Writes a string to OutputStream.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task WriteContentAsync(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            return OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Closes this response and sends it.
        /// </summary>
        public async void Close()
        {
            //try
            //{
                //await Send();

            var outputStream = OutputStream as MemoryStream;
            MemoryStream memStream = new MemoryStream(outputStream.ToArray());
            
            memStream.Seek(0, SeekOrigin.Begin);

            var socketStream = client.Stream;


            string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                            Headers +
                            $"Content-Length: {memStream.Length}\r\n" +
                            "\r\n";

            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            await socketStream.WriteAsync(headerArray, 0, headerArray.Length);
            await memStream.CopyToAsync(socketStream);

            await socketStream.FlushAsync();
            //}
            //catch (Exception ex)
            //{
            //    if (SimpleServerConfig.IgnoreSendExceptions)
            //    {
            //        Log.Warn(Request.Method + " " + Request.RawUrl + " raised a send exception. For more details, disable IgnoreSendExceptions.");
            //    }
            //    else
            //    {
            //        Log.Error(ex);
            //    }
            //}
            CloseSocket();
        }

        internal void CloseSocket()
        {
            client.Dispose();
        }

        /// <summary>
        /// Writes a HTTP redirect response.
        /// </summary>
        /// <param name="redirectLocation"></param>
        /// <returns></returns>
        public async Task RedirectAsync(Uri redirectLocation)
        {
            var outputStream = client.Stream;

            StatusCode = 301;
            ReasonPhrase = "Moved permanently";
            Headers["Location"] = redirectLocation.ToString();

            string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                            $"Location: {Headers["Location"]}" +
                            $"Content-Length: 0\r\n" +
                            "Connection: close\r\n" +
                            "\r\n";

            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            await outputStream.WriteAsync(headerArray, 0, headerArray.Length);
            await outputStream.FlushAsync();

        }

        #region IDisposable
        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Close();

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
