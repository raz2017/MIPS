using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MIPS
{
    //class UnitTests
    //{
    //    //Test random initial register values
    //    //Test random initial Main Memory Values

        

    //}

    public class TestInstructions : IInstructions
    {
        public List<int> mipsInstructions { get; set; }
    }

    [TestFixture]

    public class MainMemoryInitializationTest
    {
        
    }

    public class RegistersInitializationTest
    {
        
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


    [TestFixture]
    public class SystemRegisterTest
    {

        private IInstructions _instructionList = new TestInstructions()
        {
            mipsInstructions = new List<int>()
            {
                0x00a63820,
                0x00625022
            }
        };

        [Test]
        public void TestOneInstruction()
        {

            PipeLinedDataPath pipeLineTest = new PipeLinedDataPath(_instructionList);

            pipeLineTest.ProcessInstructions();

        }
    }


}
