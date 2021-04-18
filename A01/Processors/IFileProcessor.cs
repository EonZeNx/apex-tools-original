namespace A01
{
    /*
     * REFERENCE: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
     * REFERENCE: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
     * sbyte : int8
     * byte : uint8
     * short : int16
     * ushort : uint16
     * int : int32
     * uint : uint32
     * long : int64
     * ulong : uint64
     */
    
    public interface IFileProcessor
    {
        public void LoadBinary(string path);
        public void ExportConverted(string path);

        public void LoadConverted(string path);
        public void ExportBinary(string path);
    }
}