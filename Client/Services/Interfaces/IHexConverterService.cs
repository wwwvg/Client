namespace Client.Services.Interfaces
{
    public interface IHexConverterService
    {
        string ToHex(byte[] bytes);
        byte[] ToBytes(string hexNumbers, int lenght, string separator = " ");
    }
}