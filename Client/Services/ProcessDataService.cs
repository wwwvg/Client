using Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class ProcessDataService : IProcessDataService
    {
        public bool GetBytes(string data, out byte[] bytes, int settingsSize, out string errorMessage, IHexConverterService hexConverterService)
        {
            if(!CheckData(data, settingsSize, out string message))
            {
                errorMessage = message;
                bytes = null;
                return false;
            }
            errorMessage = "";
            bytes = hexConverterService.ToBytes(data);
            return true;
        }

        public bool CheckData(string data, int settingsSize, out string ErrorMessage)
        {
            if (data == null)
            {
                ErrorMessage = "Нет данных";
                return false;
            }

            data = data.ToUpper();
            var include = new HashSet<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', ' ' };
            foreach (var item in data)
                if (!include.Contains(item))
                {
                    ErrorMessage = $"Символ <{item}> недопустим. Разрешены только 01234567890ABCDEF";
                    return false;
                }

            data = data.Replace(" ", "");
            if (data.Length != settingsSize * 2) // если маска не до конца заполнена (были пробелы вместо значений)
            {
                ErrorMessage = "Маска заполнена не полностью либо присутствуют пробелы на месте значений";
                return false;
            }
            ErrorMessage = "";
            return true;
        }
    }
}
