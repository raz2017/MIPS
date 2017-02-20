using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.MainMemoryAndRegisters
{
    class Registers
    {
        public int[] regValues;

        public Registers()
        {
            Initialize();
        }

        private void Initialize()
        {
            regValues = new int[32];
            regValues[0] = 0;
            for (int i = 1; i < 32; i++)
                regValues[i] = (0x100 + i);
        }
    }
}
