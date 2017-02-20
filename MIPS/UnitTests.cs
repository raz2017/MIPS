using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MIPS.MainMemoryAndRegisters;

namespace MIPS
{

    public class TestInstructions : IInstructions
    {
        public List<int> mipsInstructions { get; set; }
    }

    [TestFixture]
    public class MainMemoryInitializationTest
    {
        private MainMemory mainMemory;

        public MainMemoryInitializationTest()
        {
            mainMemory = new MainMemory();
        }

        [Test]
        public void TestMainMemoryValues()
        {
            Assert.AreEqual(0, mainMemory.memValues[0]);
            Assert.AreEqual(1, mainMemory.memValues[1]);
            Assert.AreEqual(0xFF, mainMemory.memValues[0xFF]);
            Assert.AreEqual(0, mainMemory.memValues[0x100]);
            Assert.AreEqual(1, mainMemory.memValues[0x101]);
        }

    }

    [TestFixture]
    public class RegistersInitializationTest
    {
        private Registers registers;

        public RegistersInitializationTest()
        {
            registers = new Registers();
        }

        [Test]
        public void TestRegisterValues()
        {
            Assert.AreEqual(0, registers.regValues[0]);
            Assert.AreEqual(0x101, registers.regValues[1]);
            Assert.AreEqual(0x102, registers.regValues[2]);
            Assert.AreEqual(0x10a, registers.regValues[10]);
            Assert.AreEqual(0x11f, registers.regValues[31]);
        }
    }

    //Do System Wide Test First.
    [TestFixture]
    public class SystemTest
    {

        private IInstructions _instructionList;

        [Test]
        public void TestAllInstructions()
        {
            _instructionList = new Instructions();

            PipeLinedDataPath pipeLineTest = new PipeLinedDataPath(_instructionList);

            pipeLineTest.ProcessInstructions();

        }
    }


    [TestFixture]
    public class SystemOneInstructionTest
    {

        private IInstructions _instructionList;

        [Test]
        public void TestNOOPInstruction()
        {
            _instructionList = new TestInstructions()
            {
                mipsInstructions = new List<int>()
                {
                    0x00000000
                }
            };

            PipeLinedDataPath pipeLineTest = new PipeLinedDataPath(_instructionList);

            pipeLineTest.ProcessInstructions();

        }

        [Test]
        public void TestAddInstruction()
        {
            
        }

        [Test]
        public void TestSubtractInstruction()
        {
            
        }

        [Test]
        public void TestLoadByteInstruction()
        {
            
        }

        [Test]
        public void TestStoreByteInstruction()
        {
            
        }


    }

    [TestFixture]
    public class SystemTwoInstructionsTest
    {

        private IInstructions _instructionList;

        [Test]
        public void TestNOOPInstructions()
        {
            _instructionList = new TestInstructions()
            {
                mipsInstructions = new List<int>()
                {
                    0x00000000
                }
            };

            PipeLinedDataPath pipeLineTest = new PipeLinedDataPath(_instructionList);

            pipeLineTest.ProcessInstructions();

        }

        [Test]
        public void TestAddInstructions()
        {

        }

        [Test]
        public void TestSubtractInstructions()
        {

        }

        [Test]
        public void TestLoadByteInstructions()
        {

        }

        [Test]
        public void TestStoreByteInstructions()
        {

        }


    }






}
