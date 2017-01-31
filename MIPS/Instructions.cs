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
    public class Instructions
    {
        private static List<int> _instructions;

        public Instructions()
        {
            ReadInstructionsFromFile();
        }
        private void ReadInstructionsFromFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = ConfigurationManager.AppSettings["MIPSInstructions"] ;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        _instructions.Add(Convert.ToInt16(reader.Read()));
                    }
                }
            }

        }

        private static void OutputRegisterDetails()
        {
            
        }

        public List<int> mipsInstructions
        {
            get
            {
                return _instructions;
            }
        }

    }
}
