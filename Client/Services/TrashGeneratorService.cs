using Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class TrashGeneratorService : ITrashGeneratorService
    {
        public byte[] GetBytes(int amount)
        {
            byte[] bytes = new byte[amount];
            for (int i = 0; i < amount; i++)
            {
                bytes[i] = (byte)GiveMeANumber(0 , 255, 10, 11);  // 0xA = 10, 0xB = 11
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
            var range = Enumerable.Range(from, to).Where(i => !exclude.Contains(i));
            var rand = new System.Random();
            int index = rand.Next(from, to - exclude.Count);
            return range.ElementAt(index);
        }
    }
}
