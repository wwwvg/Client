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
            for(int i = 0; i < bytes.Length; i++)
            {
                string hex = bytes[i].ToString("X");
                if (hex.Length == 1)
                    hex = "0" + hex;
                if(i == bytes.Length - 1)
                    sb.Append(hex);
                else
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
                if(listOfHex[i] != "")
                    bytes[i] = Convert.ToByte(listOfHex[i], 16);
            }
            return bytes;
        }
    }
}
