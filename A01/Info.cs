namespace A01
{
    struct Info
    {
        private static string ProgramName { get; }  = "Apex Engine Tools";
        private static int MajorVersion { get; }  = 0;
        private static int MinorVersion { get; }  = 0;
        private static int BugfixVersion { get; }  = 0;

        public static string Get()
        {
            return $"{ProgramName} v{MajorVersion}.{MinorVersion}.{BugfixVersion}";
        }
    }
}