using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Logging
{
    public class Logger
    {
        private StreamWriter LogFile { get; set; }
        private TextWriter InitialLog { get; set; }
        private string prefix { get; set; }
        public string Prefix { get { return prefix; } }
        private Logger() { }
        public static Logger CreateFrom(TextWriter initialLogger,string output,string prefix)
        {
            Logger logger = new Logger();
            logger.InitialLog = initialLogger;
            logger.LogFile = new StreamWriter(output,true) { AutoFlush = true };
            logger.prefix = " [" + prefix + "] ";
            return logger;
        }
        public static Logger CreateFrom(TextWriter initialLogger, string output)
        {
            Logger logger = new Logger();
            logger.InitialLog = initialLogger;
            logger.LogFile = new StreamWriter(output) { AutoFlush = true };
            logger.prefix = "";
            return logger;
        }
        public async void INFO(string s)
        {
            await InitialLog.WriteLineAsync("[INFO] "+DateTime.Now.ToString()+Prefix+s);
            await LogFile.WriteLineAsync("[INFO] " + DateTime.Now.ToString() + Prefix + s);
        }
        public async void WARN(string s)
        {
            await InitialLog.WriteLineAsync("[WARN] " + DateTime.Now.ToString() + Prefix + s);
            await LogFile.WriteLineAsync("[WARN] " + DateTime.Now.ToString() + Prefix + s);
        }
        public async void ERROR(Exception e,string message)
        {
            await InitialLog.WriteLineAsync("[ERROR] " + DateTime.Now.ToString() + Prefix + message);
            await InitialLog.WriteLineAsync("[ERROR] " + DateTime.Now.ToString() + Prefix + e.ToString());
            await LogFile.WriteLineAsync("[ERROR] " + DateTime.Now.ToString() + Prefix + message);
            await LogFile.WriteLineAsync("[ERROR] " + DateTime.Now.ToString() + Prefix + e.ToString());
        }
    }
}
