using System;

namespace A01.Utils
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