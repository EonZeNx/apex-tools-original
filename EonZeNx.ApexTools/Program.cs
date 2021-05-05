using System;
using System.IO;
using System.Linq;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools
{
    class Program
    {
        private static readonly string[] MOTDS = {
            "Now featuring ∞ more C#!",
            "Now featuring ∞ less Python!",
            "More speed, less not-speed",
            "fuck this took way too long to make",
            "This stupid random MOTD was one of the first things I did",
        };

        public static ConfigData Config;

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

        private static void LoadConfig()
        {
            Config = new ConfigData();
            Config.Load();
        }
        
        static void Main(string[] args)
        {
            Console.Title = Info.Get();
            LoadConfig();
            WriteMOTD();

            // Program launches with itself as the first argument, filter it
            var filepaths = args[1..].Where(File.Exists).ToArray();
            if (args.Length > 0)
            {
                var manager = new FileManager(filepaths);
                manager.ProcessPaths();
            }

            // No auto close optional argument
            if (!Config.AutoClose)
            {
                ConsoleUtils.GetInput("Press any key to continue...");
            }
        }
    }
}