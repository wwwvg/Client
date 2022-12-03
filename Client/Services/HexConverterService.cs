using Client.Services.Interfaces;
using System;
using System.Collections.Generic;
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

        public byte[] ToBytes(string hexNumbers, int lenght, string separator = " ")
        {
            if (hexNumbers.Replace(" ", "") == String.Empty || lenght == 0)
                return null;

            string compressed = hexNumbers.Replace(" ", "").Replace(",", "").Replace("/", "").Replace(".", "").Replace("*", "");
            if(compressed.Length % 2 != 0)
                return null;

            var queue = new Queue<char>();
            foreach(var s in compressed)
                queue.Enqueue(s);

            var bytes = new byte[lenght];
            for (int i = 0; i < lenght; i++)
            {

                try
                {
                    string ab = $"{queue.Dequeue()}{queue.Dequeue()}";
                    bytes[i] = Convert.ToByte(ab, 16);
                }
                catch (Exception)
                {
                    return null;
                }

            }
            return bytes;
        }
    }
}
