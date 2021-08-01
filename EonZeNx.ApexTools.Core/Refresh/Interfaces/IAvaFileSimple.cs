namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IAvaFileSimple
    {
        void Deserialize(byte[] contents);
        byte[] Export();
    }
}