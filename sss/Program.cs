using System;

namespace SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SimpleServer");
            Console.WriteLine("Version v"+VersionInfo.Version);
            Console.WriteLine();
            if (args == null || args.Length == 0)
            {
                WriteHelp();
                Console.ResetColor();
                Environment.Exit(1);
            }
        }
        static void WriteHelp()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Usage: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("<module>");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" [options]");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Modules:");
            Console.WriteLine("  --server: The module that allows you to host a (HTTP) server");
            Console.WriteLine("    Options for the server module:");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write    ("      --port");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(": the TCP port to host this server on");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write    ("      --file");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(": the server file that contains the functions of the server");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write    ("      --gui-port");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(": The port of the GUI host, created by SimpleServer");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("<Required Parameters>");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" [Optional Parameters]");
        }
    }
}