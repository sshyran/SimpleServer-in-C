using DMP9Labs.IO.Plugins;
using SimpleServer.Net;
using SimpleServer.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace SimpleServer
{
    public static class SimpleServer
    {
        public static List<PluginBase> CachedPlugins { get; set; }
        public static Dictionary<PluginBase, Logger> PluginLoggerRegistry { get; set; }
        public static Server OpenServer { get; set; }
        public static Logging.Logger Logger { get; set; }
        public static string DefaultErrorPage { get { return Internals.ErrorPage; } }
        public static string APIVersion { get { return typeof(SimpleServer).Assembly.GetName().Version.Major.ToString() + typeof(SimpleServer).Assembly.GetName().Version.MajorRevision; } }
    }
    [Serializable]
    public abstract class Function : ISerializable
    {
        public abstract string Data { get; set; }
        public abstract string Path { get; set; }
        public abstract List<string> AuthorizedAccounts { get; set; }
        public abstract bool AccessLockdown { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
    }
    public class SSProgressEventArgs : EventArgs
    {

    }
    public class AccountCollection : Collection<Account>
    {
        private class SResult
        {
        }
        public Account this[string s]
        {
            get
            {
                try
                {
                    return this.Select(x => x[s]).Where(x => x != null).First();
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetItem(IndexOf(this.Select(x => x[s]).Where(x => x != null).First()),value);
            }
        }
        public new void Add(Account a)
        {
            if (this[a.Username]!=null)
            {
                throw new AccountDuplicateException();
            }
            base.Add(a);
        }
    }
    public class AccountDuplicateException : Exception
    {
        public AccountDuplicateException()
            : base("An existing account object with the same username is already in the collection") { }
    }
    public class Server
    {
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public X509Certificate SSLCertificate { get; set; }
        public TcpListener InnerListener { get; set; }
        public bool Listening { get; private set; }
        public event EventHandler<SSProgressEventArgs> SaveProgress;
        public string Title { get; set; }
        public void Save(string to)
        {
            string tempFolder = Encoding.ASCII.GetString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(to)))+new Random().Next(int.MinValue,int.MaxValue);
            if (tempFolder.Length > 16) { tempFolder = tempFolder.Substring(0,16); }
            Directory.CreateDirectory(tempFolder);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.XsdString;
            ZipArchive z = new ZipArchive(new FileStream(to,FileMode.OpenOrCreate), ZipArchiveMode.Update);
            foreach (Function f in Functions)
            {
                string name = tempFolder + "\\" + Encoding.ASCII.GetString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(f.Path))).Substring(0, 16) + ".dlf";
                FileStream fs = new FileStream(name,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);
                formatter.Serialize(fs,f);
                fs.Flush();
                fs.Dispose();
                var entry = z.CreateEntryFromFile(name, Path.GetFileName(name), CompressionLevel.Optimal);
            }
            FileStream acc = new FileStream(tempFolder + "\\" + Encoding.ASCII.GetString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes("accounts"))).Substring(0, 16) + ".dlf",FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(acc);
            foreach (Account a in Accounts)
            {
                sw.Write(a.Username+":"+a.Password+"!");
            }
            sw.Flush();
            sw.Close();
            var entry2 = z.CreateEntryFromFile(acc.Name, Path.GetFileName(acc.Name), CompressionLevel.Optimal);
            
        }
        public List<Account> Accounts { get; set; }
        public Server(string path)
        {
        }
        public Server(Stream stream)
        {

        }
        public void Start()
        {
            Listening = true;
            Task.Factory.StartNew(() => Listen());
        }
        private async void Listen()
        {
            while (Listening)
            {
                try
                {
                    TcpClient client = await InnerListener.AcceptTcpClientAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {

                }
            }
        }
        private void Callback()
        {

        }
        public List<Function> Functions { get; set; }
    }
    [Serializable()]
    public class Account
    {
        /// <summary>
        /// The account username
        /// </summary>
        public string Username { get; private set; }
        /// <summary>
        /// The account password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Returns the account if the Account instance matches the username, otherwise returns null
        /// </summary>
        /// <param name="s">the username to check</param>
        /// <returns>the account if the account has the same username as s</returns>
        public Account this[string s]
        {
            get { if (this.Username == s) { return this; } else { return null; } }
        }
    }
}