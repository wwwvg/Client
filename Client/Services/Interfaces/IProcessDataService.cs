using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public interface IProcessDataService
    {
        bool GetBytes(string data, out byte[] bytes, int settingsSize, out string ErrorMessage);
        bool CheckData(string data, int settingsSize, out string ErrorMessage);
    }
}
