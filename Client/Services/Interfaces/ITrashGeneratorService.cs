﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.Interfaces
{
    public  interface ITrashGeneratorService
    {
        byte[] GetBytes(int amount);
    }
}
