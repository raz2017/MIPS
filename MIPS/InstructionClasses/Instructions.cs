using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MIPS.Properties;

namespace MIPS
{
    public class Instructions : IInstructions
    {
        private static List<int> _instructions = new List<int>();

        public Instructions()
        {
            ReadInstructionsFromFile();
        }
        private void ReadInstructionsFromFile()
        {
            string instructionsFromFile = Resources.Instructions;
            using (var reader = new StringReader(instructionsFromFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    _instructions.Add(Convert.ToInt32(line, 16));
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
                //return _instructions;
                return _instructions;
            }
        }

    }
}
