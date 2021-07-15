using System;
using System.IO;
using System.Linq;
using EonZeNx.ApexTools.Configuration;
using EonZeNx.ApexTools.Core;
using EonZeNx.ApexTools.Core.Utils;

namespace EonZeNx.ApexTools
{
    public static class Motd
    {
        private static readonly string[] Motds = {
            "Now featuring ∞ more C#!",
            "Now featuring ∞ less Python!",
            "More speed, less not-speed",
            "fuck this took way too long to make",
            "This stupid random MOTD was one of the first things I did",
        };
        
        /// <summary>
        /// Push a little message to the top of the console.
        /// </summary>
        public static void WriteMotd()
        {
            // Message of the Day
            var randomIntMotd = new Random().Next(0, Motds.Length);
            var randomMotd = Motds[randomIntMotd];
            
            var motd = $"{Info.Get()} - {randomMotd}";
            var motdBreak = new string('=', motd.Length);
            Console.WriteLine(motdBreak);
            Console.WriteLine(motd);
            Console.WriteLine(motdBreak);
        }
    }
    
    
    class Program
    {

        private static void Close(string msg = "")
        {
            if (msg.Length != 0) Console.WriteLine(msg);
            
            // No auto close optional argument
            if (!ConfigData.AutoClose) ConsoleUtils.GetInput("Press any key to continue...");

            Environment.Exit(0);
        }

        /// <summary>
        /// Arguments passed to the application may include invalid targets.
        /// This will ensure invalid targets, such as .exe or malformed paths are removed.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string[] FilterFilepaths(string[] args)
        {
            var filepaths = args
                .Where(path => File.Exists(path) || Directory.Exists(path))
                .Where(path => !path.Contains(".exe")).ToArray();

            return filepaths;
        }
        
        private static void Main(string[] args)
        {
            Console.Title = Info.Get();
            ConfigData.Load();
            Motd.WriteMotd();

            if (args.Length == 0)
            {
                Close("No arguments passed. Try dragging a supported file onto me.");
                return;
            }
            
            // Program launches with itself as the first argument, filter it
            var filepaths = FilterFilepaths(args);
            if (filepaths.Length == 0) Close("No valid paths detected. Double check the paths used.");
            
            var manager = new AvaFileTypeManager(filepaths);
            manager.ProcessPaths();
            Close();
        }
    }
}