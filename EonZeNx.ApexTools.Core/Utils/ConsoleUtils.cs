using System;

namespace EonZeNx.ApexTools.Core.Utils
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