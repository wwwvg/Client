namespace Client.Services.Interfaces
{
    public interface IBytesGeneratorService
    {
        byte[] GetBytes(int trashLenght1, byte[] data, int trashLenght2);
        byte[] GetBytes(int lenght);
    }
}
