using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MIPS
{
    class UnitTests
    {
        //Test random initial register values
        //Test random initial Main Memory Values

        

    }

    [TestFixture]
    public class DecodeTest
    {
        private List<int> instructions;

        public DecodeTest()
        {
            instructions = new List<int>()
            {
               0x00a63820,
               0x00625022
            };
        }

        [Test]
        public void TestRFormat()
        {
            foreach (var instruction in instructions)
            {
                var inst = InstructionDecoder.is_r_format(instruction);
                Assert.AreEqual(inst,true);
            }
        }
    }
}
