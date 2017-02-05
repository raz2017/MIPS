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
    public class Instructions
    {
        private static List<int> _instructions = new List<int>();

        public Instructions()
        {
            ReadInstructionsFromFile();
        }
        private void ReadInstructionsFromFile()
        {
          //  ReadInstructionsFromFile2();
            string InstructionsFromFile = Resources.Instructions;
            using (var reader = new StringReader(InstructionsFromFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    _instructions.Add(Convert.ToInt32(line, 16));
                }
            }

            // //Stream stream1 = assembly.GetManifestResourceStream(result[0]);
            //// StreamReader reader1 = new StreamReader(stream1);
            //// _instructions.Add(reader1.Read().ToString());
            // Stream stream2 = assembly.GetManifestResourceStream(result[1]);
            // StreamReader reader2 = new StreamReader(stream2);
            // _instructions.Add(reader2.Read().ToString());
            // Stream stream3 = assembly.GetManifestResourceStream(result[2]);
            // StreamReader reader3 = new StreamReader(stream3);
            // _instructions.Add(reader3.Read().ToString());




        }

 //       public static void ReadInstructionsFromFile2()
 //       {
 //           var assembly = Assembly.GetExecutingAssembly();
 //           var resourceName = ConfigurationManager.AppSettings["MIPSInstructions"];
 ////           List<int> instructions = new List<int>();



 //           using (Stream stream = assembly.GetManifestResourceStream(resourceName))
 //           {
 //               using (StreamReader reader = new StreamReader(stream))
 //               {
 //                   while (!reader.EndOfStream)
 //                   {
 //                       _instructions.Add(Convert.ToInt32(reader.Read().ToString(),16));
 //                   }
 //               }
 //           }

 //       }

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
