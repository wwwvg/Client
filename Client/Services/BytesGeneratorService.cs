using Client.Services.Interfaces;
using Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class BytesGeneratorService : IBytesGeneratorService
    {
        //public byte[] GetBytes(int amount)
        //{
        //    byte[] bytes = new byte[amount];
        //    for (int i = 0; i < amount; i++)
        //    {
        //        bytes[i] = (byte)GiveMeANumber(0 , 255, 10, 11);  // 0xA = 10, 0xB = 11   -   стартовый и стоповый байты
        //    }
        //    return bytes;
        //}

        public byte[] GetBytes(int trashLenght1, byte[] data, int trashLenght2)
        {
            if(data == null)
                return null;

            int totalLenght = trashLenght1 + data.Length + trashLenght2;
            if (totalLenght > ContentViewModel.MaxBytes)
                return null;

            byte[] bytes = new byte[totalLenght];

            for (int i = 0; i < trashLenght1; i++)      // мусор в начале
                bytes[i] = (byte)GiveMeANumber(0, 255, 10, 11);  // 0xA = 10, 0xB = 11  

            for (int i = trashLenght1; i < data.Length + trashLenght1; i++) // данные
            {
                bytes[i] = data[i-trashLenght1];
            }

            for (int i = trashLenght1 + data.Length; i < trashLenght2 + trashLenght1 + data.Length; i++)   // мусор в конце
                bytes[i] = (byte)GiveMeANumber(0, 255, 10, 11);  // 0xA = 10, 0xB = 11

            return bytes;
        }

        public byte[] GetBytes(int lenght)
        {
            byte[] bytes = new byte[lenght];

            for (int i = 0; i < lenght; i++) // данные
            {
                bytes[i] = (byte)GiveMeANumber(0, 255);
            }
            return bytes;
        }

        private int GiveMeANumber(int from, int to, params int[] prohibitedSymbols)
        {
            var exclude = new HashSet<int>();
            foreach (var item in prohibitedSymbols)
            {
                exclude.Add(item);
            }
            var range = Enumerable.Range(from, to).Where(i => !exclude.Contains(i));  // дать случайное число из диапазона, за исключением prohibitedSymbols
            var rand = new System.Random();
            int index = rand.Next(from, to - exclude.Count);
            return range.ElementAt(index);
        }
    }
}
