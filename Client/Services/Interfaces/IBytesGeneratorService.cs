using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public  interface IBytesGeneratorService
    {
        byte[] GetBytes(int trashLenght1, byte[] data, int trashLenght2);
    }
}
