using System;
using System.Collections.Generic;
using System.IO;
using System.Dynamic;
using System.Linq;
using A01.Utility;

namespace A01
{
    class Program
    {
        private static Dictionary<string, string> arguments = new ()
        {
            {"-h", "Help: Display this message. All arguments are lower case."},
            {"-nac", "No Auto Close: Will prevent the console from automatically closing."}
        };

        private static void ParseArguments(string[] args)
        {
            // No args or -h passed
            if (args.Length == 0 || arguments.ContainsKey("-h") || arguments.ContainsKey("-help"))
            {
                foreach (KeyValuePair<string, string> entry in arguments)
                {
                    Console.Write(entry.Key);
                    Console.Write("\t");
                    Console.Write(entry.Value);
                    Console.WriteLine();
                }
            }
            
            // To add other args, create a class variable and set it when detecting the argument.
        }
        
        static void Main(string[] args)
        {
            // Message of the Day
            Console.Title = Info.Get();
            
            var motd = $"{Info.Get()} - Now featuring ∞ more C#!";
            var motdBreak = new string('=', motd.Length);
            Console.WriteLine(motdBreak);
            Console.WriteLine(motd);
            Console.WriteLine(motdBreak);
            
            ParseArguments(args);
            
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            // No auto close optional argument
            if (args.Any(arg => arg == "-nac") || args.Length == 0)
            {
                ConsoleUtils.GetInput("Press any key to continue...");
            }
        }
    }

    class Info
    {
        public static string ProgramName { get; }  = "Apex Engine Tools";
        public static int MajorVersion { get; }  = 0;
        public static int MinorVersion { get; }  = 0;
        public static int BugfixVersion { get; }  = 0;

        public static string Get()
        {
            return $"{ProgramName} v{MajorVersion}.{MinorVersion}.{BugfixVersion}";
        }
    }
}