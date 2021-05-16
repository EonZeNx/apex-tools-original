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

        private static void Close(string msg = "")
        {
            if (msg.Length != 0) Console.WriteLine(msg);
            
            // No auto close optional argument
            if (!ConfigData.AutoClose)
            {
                ConsoleUtils.GetInput("Press any key to continue...");
            }
        }
        
        static void Main(string[] args)
        {
            Console.Title = Info.Get();
            ConfigData.Load();
            WriteMOTD();

            if (args.Length == 0)
            {
                Close("No arguments passed. Try dragging a supported file onto me.");
                return;
            }
            
            // Program launches with itself as the first argument, filter it
            var filepaths = args
                .Where(File.Exists)
                .Where(path => !path.Contains(".exe")).ToArray();
            if (filepaths.Length == 0)
            {
                Close("No valid paths detected. Double check the paths used.");
                return;
            }
            
            var manager = new FileManager(filepaths);
            manager.ProcessPaths();
            Close();
        }
    }
}