using System;

namespace A01.Utility
{
    public static class ConsoleUtils
    {
        public static string GetInput(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }
    }
}