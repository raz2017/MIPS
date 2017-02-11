using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


//TODO Come up with a good design for the classes
//TODO Create unit tests
namespace MIPS
{
    class Program
    {
        static void Main(string[] args)
        {
             
            PipeLinedDataPath pipe = new PipeLinedDataPath(new Instructions());
            pipe.ProcessInstructions();
        }
    }
}
