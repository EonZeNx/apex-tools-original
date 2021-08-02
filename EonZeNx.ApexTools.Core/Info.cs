namespace EonZeNx.ApexTools.Core
{
    public struct Info
    {
        private static string ProgramName => "Apex Engine Tools";
        private static int MajorVersion => 0;
        private static int MinorVersion => 5;
        private static int BugfixVersion => 1;

        public static string Get()
        {
            return $"{ProgramName} v{MajorVersion}.{MinorVersion}.{BugfixVersion}";
        }
    }
}