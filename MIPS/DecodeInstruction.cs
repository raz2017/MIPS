using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS
{
    public static class DecodeInstruction
    {
        public static string rfunct(int x)
        {
            switch (x & 0x0000003f)
            {
                case 0x22:
                    return "sub";
                case 0x20:
                    return "add";
                case 0x00:
                    return "nop";
                default:
                    return "NotDef";
            }
        }

        public static string getOPcode(int x)
        {
            switch ((x & 0xFC000000) >> 26)
            {
                case 0x20:
                    return "lb";
                case 0x28:
                    return "sb";
                default:
                    return "NotDef";
            };
        }

        public static bool is_r_format(int x)
        {
            return (x & 0xFC000000) == 0 ? true : false;
        }

        public static int source_reg1(int x)
        {
            return (x & 0x03e00000) >> 21;
        }

        public static int source_reg2(int x)
        {
            return (x & 0x001f0000) >> 16;
        }

        public static int dest_reg(int x)
        {
            return (x & 0x0000f800) >> 11;
        }

        public static short offset(int x)
        {
            return (short)(x & 0x0000ffff);
        }

       public static int func_code(int x)
        {
            return x & 0x0000003f;
        }
    }
}
