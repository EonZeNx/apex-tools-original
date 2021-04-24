using System;
using System.Collections.Generic;
using System.Linq;
using A01.Configuration;
using A01.Processors;
using A01.Utils;
using Microsoft.Extensions.Configuration;

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

        public static IConfiguration Config;

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
            Config = new ConfigurationBuilder()
                .AddJsonFile(@"appsettings.json")
                .Build();
            
            WriteMOTD();
            
            var filepaths = FilterForFilePaths(args);
            var manager = new FileManager(filepaths);
            manager.ProcessPaths();

            // No auto close optional argument
            if (!Config.GetAutoClose())
            {
                ConsoleUtils.GetInput("Press any key to continue...");
            }
        }
    }
}