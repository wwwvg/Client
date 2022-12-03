namespace Client.Services.Interfaces
{
    public interface IHexConverterService
    {
        string ToHex(byte[] bytes);
        byte[] ToBytes(string hexNumbers, string separator = " ");
    }
}