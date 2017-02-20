using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.MainMemoryAndRegisters;
using MIPS.PipelineStageClasses;


namespace MIPS
{
    public class PipeLinedDataPath
    {
        private MainMemory mainMemory;
        private Registers registers;

        private int _instructionIndex;

        private PipeLineStagesContainer pipelineStage;

        private readonly IInstructions _instructions;

        public PipeLinedDataPath(IInstructions instructions)
        {
        
            pipelineStage = new PipeLineStagesContainer(instructions);

            InitializeMainMemory();
            InitializeRegisters();

            _instructions = instructions;
            _instructionIndex = 0;
        }
        /// <summary>
        /// Read next instruction from instruction cache.
        /// Put it in the WRITE version of the IF/ID pipeline register.
        /// </summary>
        public void IF_FetchInstructions()
        {
            
            pipelineStage.ifid_write.FetchInstruction(_instructionIndex);
            _instructionIndex++;

        }
        
        /// <summary>
        /// 1. Read instruction from the READ version of the IF/ID pipeline register.
        /// 2. Do the decoding and register fetching
        /// 3. Write the values to the WRITE version of the ID/EX pipeline register
        /// </summary>
        public void ID_DecodeInstructions()
        {
           //No control signals here, we decode instruction
           //Figure out what the control bits are going to be for instruction

            pipelineStage.idex_write.SetRegisterValues(pipelineStage.ifid_read.CurrentInstruction,registers.regValues);
            pipelineStage.idex_write.SetExecutionPath(pipelineStage.ifid_read.CurrentInstruction);

        }

        /// <summary>
        /// 1. Perform the requested instruction on the operands we got from the READ
        /// version of the IDEX pipeline register
        /// 2. Then write the appropripate values to the WRITE version of the EX/MEM pipeline
        /// register.
        /// </summary>
        public void EX_ExecuteInstructions()
        {
            //Control signals

            pipelineStage.exmem_write.SetWriteRegisterNumber(pipelineStage.idex_read); 

            //ALUSrc

            int SecondALUOperand = 0;
            SecondALUOperand = pipelineStage.idex_read._ALUSrc == 1
                ? pipelineStage.idex_read.SEOffset
                : pipelineStage.idex_read.ReadReg2Value;
            

            //ALUOp

            int aluControlInput = 0;
            aluControlInput = pipelineStage.idex_read.GetALUControlInput();

            //ALU
            pipelineStage.exmem_write.SetALUResult(pipelineStage.idex_read, SecondALUOperand , aluControlInput);

            //Store word value just pass values from IDEX Read pipeline

            pipelineStage.exmem_write.SetMemoryControlsBits(pipelineStage.idex_read);

        }

        /// <summary>
        /// 1. If the instruction is a "load byte", the use the address calculated in the previous
        /// stage as an index into the Main Memory array and get value. 
        /// 2. Otherwise, just pass information from the read version of the previous stage (EX/MEM) pipeline register
        /// to the WRITE version fof the MEM/WB.
        /// </summary>
        public void MEM_LoadValueFromMemory()
        {
            //Control Signals
            //R-Type should be NOP

            //Read from Main Memory
            if (pipelineStage.exmem_write.MemRead == 1)
                pipelineStage.memwb_write.LWDataValue = mainMemory.memValues[pipelineStage.exmem_read.ALUResult];

            //Write to Main Memory
            if (pipelineStage.exmem_read.MemWrite == 1)
                mainMemory.memValues[pipelineStage.exmem_read.ALUResult] = pipelineStage.exmem_read.SWvalue;

            pipelineStage.memwb_write.SetWriteBackValues(pipelineStage.exmem_read);
        }
        
        /// <summary>
        /// Write to the registers based on information we read from the READ version of MEM/WB
        /// </summary>
        public void WB_StoreValuesToRegisters()
        {
            //SB shouldn't do anything here
            if (pipelineStage.memwb_read.RegWrite == 1)

                registers.regValues[pipelineStage.memwb_read.WriteRegNum] = pipelineStage.memwb_read.MemToReg == 1
                    ? pipelineStage.memwb_read.LWDataValue
                    : pipelineStage.memwb_read.ALUResult;
            
        }

        private void InitializeMainMemory()
        {
            mainMemory = new MainMemory();
        }

        private void InitializeRegisters()
        {
            registers = new Registers();
           
        }

        public void ProcessInstructions()
        {
            foreach (var instruction in  _instructions.mipsInstructions)
            {
                IF_FetchInstructions();
                ID_DecodeInstructions();
                EX_ExecuteInstructions();
                MEM_LoadValueFromMemory();
                WB_StoreValuesToRegisters();

                PrintRegisterValues();
                CopyWriteToReadValues();

            }
        }

        private void PrintRegisterValues()
        {
            //Todo after writing unit and system tests
        }

        private void CopyWriteToReadValues()
        {
            pipelineStage.CopyWriteToReadValues();
        }

    }
}
