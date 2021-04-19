using System;
using System.Collections.Generic;
using System.IO;
using System.Dynamic;
using System.Linq;
using A01.Processors.IRTPC;
using A01.Utils;

namespace A01
{
    class Program
    {
        private static readonly string[] MOTDS = {
            "Now featuring ∞ more C#!",
            "Now featuring ∞ less Python!",
            "More speed, less not-speed",
            "Geht to da chapper!",
            "fuck this took way too long to make",
            "This stupid random MOTD was one of the first things I did",
        };
        
        private static readonly Dictionary<string, string> arguments = new () {
            {"-h", "Help: Display this message. All arguments are lower case."},
            {"-nac", "No Auto Close: Will prevent the console from automatically closing."}
        };

        private static bool autoClose = true;

        /// <summary>
        /// Push a little message to the top of the console.
        /// </summary>
        private static void WriteMOTD()
        {
            // Message of the Day
            var randomIntMOTD = new Random().Next(0, MOTDS.Length);
            var randomMOTD = MOTDS[randomIntMOTD];
            
            var motd = $"{Info.Get()} - {randomMOTD}";
            var motdBreak = new string('=', motd.Length);
            Console.WriteLine(motdBreak);
            Console.WriteLine(motd);
            Console.WriteLine(motdBreak);
        }

        /// <summary>
        /// Parse arguments and set the code variables for them
        /// </summary>
        /// <param name="args">Args launched with program.</param>
        private static void ParseArguments(string[] args)
        {
            // No args or -h passed
            if (args.Length == 0 || args.Contains("-h") || args.Contains("-help"))
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
            if (arguments.ContainsKey("-nac")) autoClose = false;
        }

        /// <summary>
        /// Filters out arguments passed to program. This should be an array of filepaths.
        /// </summary>
        /// <param name="args">Args launched with program.</param>
        /// <returns></returns>
        private static string[] FilterForFilePaths(string[] args)
        {
            return args.Where(arg => !arguments.ContainsKey(arg)).ToArray();
        }
        
        static void Main(string[] args)
        {
            Console.Title = Info.Get();
            
            WriteMOTD();
            ParseArguments(args);
            
            var filepaths = FilterForFilePaths(args);
            var manager = new FileManager(filepaths);
            manager.ProcessPaths();

            // No auto close optional argument
            if (!autoClose)
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