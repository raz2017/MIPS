using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.MainMemoryAndRegisters
{
    class MainMemory
    {
        public int[] memValues;

        public MainMemory()
        {
            Initialize();
        }

        private void Initialize()
        {
            memValues = new int[1024];
            for (int i = 0; i < 1024; i++)
                memValues[i] = (i & 0x0FF);
        }
    }
}
