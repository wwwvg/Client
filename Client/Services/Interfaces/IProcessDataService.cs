namespace Client.Services.Interfaces
{
    public interface IProcessDataService
    {
        bool GetBytes(string data, out byte[] bytes, int settingsSize, out string errorMessage, IHexConverterService hexConverterService);
        bool CheckData(string data, int settingsSize, out string ErrorMessage);
    }
}
