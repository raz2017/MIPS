using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MIPS
{
    public static class InstructionsIO
    {
        public static void ReadInstructionsFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = ConfigurationManager.AppSettings["MIPSInstructions"] ;
            List<int> instructions = new List<int>();

        

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        instructions.Add(Convert.ToInt16(reader.Read()));
                    }
                }
            }

        }

        private static void OutputRegisterDetails()
        {
            
        }
    }
}
