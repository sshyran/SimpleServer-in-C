using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Internals
{
    public class SimpleServerResponse
    {
        private readonly SimpleServerConnection client;

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


        private SimpleServerRequest Request { get; }

        /// <summary>
        ///     Gets the headers of the HTTP response.
        /// </summary>
        public Dictionary<string, string> Headers { get; }

        /// <summary>
        ///     Gets the stream containing the content of this response.
        /// </summary>
        public Stream OutputStream { get; }

        /// <summary>
        ///     Gets or sets the HTTP version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP reason phrase.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        ///     Writes a string to OutputStream.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task WriteContentAsync(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            return OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Closes this response and sends it.
        /// </summary>
        public void Close()
        {
            //try
            //{
            //await Send();

            var outputStream = OutputStream as MemoryStream;
            var memStream = new MemoryStream(outputStream.ToArray());

            memStream.Seek(0, SeekOrigin.Begin);

            var socketStream = client.Stream;


            /*var header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                         Headers +
                         $"Content-Length: {memStream.Length}\r\n" +
                         "\r\n";*/
            var header = new StringBuilder();
            header.Append(Version + " " + StatusCode +" " + ReasonPhrase + "\r\n");
            Headers["Content-Length"] = memStream.Length.ToString();
            foreach (var h in Headers)
            {
                header.Append(h.Key+": "+h.Value+"\r\n");
            }

            header.Append("\r\n");
            var headerArray = Encoding.UTF8.GetBytes(header.ToString());
            socketStream.Write(headerArray, 0, headerArray.Length);
            memStream.CopyTo(socketStream);

            socketStream.Flush();
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
        ///     Writes a HTTP redirect response.
        /// </summary>
        /// <param name="redirectLocation"></param>
        /// <returns></returns>
        public async Task RedirectAsync(Uri redirectLocation)
        {
            var outputStream = client.Stream;

            StatusCode = 301;
            ReasonPhrase = "Moved permanently";
            Headers["Location"] = redirectLocation.ToString();

            var header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                         $"Location: {Headers["Location"]}" +
                         $"Content-Length: 0\r\n" +
                         "Connection: close\r\n" +
                         "\r\n";

            var headerArray = Encoding.UTF8.GetBytes(header);
            await outputStream.WriteAsync(headerArray, 0, headerArray.Length);
            await outputStream.FlushAsync();
        }

        #region IDisposable

        private bool disposedValue;

        private void Dispose(bool disposing)
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