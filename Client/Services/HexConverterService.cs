using Client.Services.Interfaces;
using System;
using System.Text;

namespace Client.Services
{
    public class HexConverterService : IHexConverterService
    {
        public string ToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in bytes)
            {
                string hex = item.ToString("X");
                if (hex.Length == 1)
                    hex = "0" + hex;
                sb.Append(hex + " ");
            }
            return sb.ToString().Trim();
        }

        public byte[] ToBytes(string hexNumbers, string separator = " ")
        {
            var listOfHex = hexNumbers.Split(separator);
            byte[] bytes = new byte[listOfHex.Length];
            for (int i = 0; i < listOfHex.Length; i++)
            {
                bytes[i] = Convert.ToByte(listOfHex[i], 16);
            }
            return bytes;
        }
    }
}
