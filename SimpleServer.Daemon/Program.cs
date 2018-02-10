using System;
using System.Diagnostics;
using System.IO;

namespace SimpleServer.Daemon
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("SimpleServer");
            Console.WriteLine("Copyright (C) Ultz");
            Console.WriteLine();
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: simpleserver {option} [parameters]");
                Console.WriteLine("{Required} [Required Parameter] ");
                Console.WriteLine();
                Console.WriteLine("run: Runs SimpleServer, assuming the current directory has a SimpleServer directory structure.");
                Console.WriteLine();
                Console.WriteLine("start: Starts SimpleServer as a background process. If a daemon/service has not been installed for SimpleServer, it will attempt to install it.");
                Console.WriteLine();
                Console.WriteLine("stop: Stops the SimpleServer background process, if present.");
                Console.WriteLine();
                Console.WriteLine("get: The root command for the SimpleServer package manager.");
                Console.WriteLine("  Download a package: simpleserver get [package name] ");
                Console.WriteLine("  Add a package repository: simpleserver get repo [repo url]");
                Console.WriteLine("  Install an offline package: simpleserver get install [package location]");
                Console.WriteLine();
                Console.WriteLine("  All package installations are irreversable, you'll need to manually delete the files.");
            }
            else if (args[0] == "run")
            {
                Console.WriteLine("Looking for hosts...");
            }
        }
    }
}
