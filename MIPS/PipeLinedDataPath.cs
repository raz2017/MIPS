using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.PipelineStageClasses;

namespace MIPS
{
    public class PipeLinedDataPath
    {
        private int[] _mainMemoryStorage;
        private int[] _registers;
        private int _instructionIndex;

        private PipeLineStagesContainer pipelineStage;

        private readonly Instructions _instructions;

        public PipeLinedDataPath(Instructions instructions)
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

            pipelineStage.idex_write.SetRegisterValues(pipelineStage.ifid_read.CurrentInstruction,_registers );
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
        /// 1. If the instruction is a "load byte", the user the address calculated in the previous
        /// stage as an index into the Main Memory array and get value. 
        /// 2. Otherwise, just pass information from the read version of the previous stage (EX/MEM) pipeline register
        /// to the WRITE version fof the MEM/WB.
        /// </summary>
        public void MEM_LoadValueFromMemory()
        {
            //Control Signals
            //R-Type should be NOP
            if (pipelineStage.exmem_write.MemRead == 1)
                pipelineStage.memwb_write.LWDataValue = _mainMemoryStorage[pipelineStage.exmem_read.ALUResult];

            if (pipelineStage.exmem_read.MemWrite == 1)
                _mainMemoryStorage[pipelineStage.exmem_read.ALUResult] = pipelineStage.exmem_read.SWvalue;


            pipelineStage.memwb_write.RegWrite = pipelineStage.exmem_read.RegWrite;
            pipelineStage.memwb_write.MemToReg = pipelineStage.exmem_read.MemToReg;

            pipelineStage.memwb_write.ALUResult = pipelineStage.exmem_read.ALUResult;
            pipelineStage.memwb_write.WriteRegNum = pipelineStage.exmem_read.WriteRegNum;

        }
        
        /// <summary>
        /// Write to the registers based on information we read from the READ version of MEM/WB
        /// </summary>
        public void WB_StoreValuesToRegisters()
        {
            //SB shouldn't do anything here
            if (pipelineStage.memwb_read.RegWrite == 1)
            {
                if (pipelineStage.memwb_read.MemToReg == 0)
                {
                    _registers[pipelineStage.memwb_read.WriteRegNum] = pipelineStage.memwb_read.ALUResult;
                }
                if (pipelineStage.memwb_read.MemToReg == 1)
                {
                    _registers[pipelineStage.memwb_read.WriteRegNum] = pipelineStage.memwb_read.LWDataValue;
                }
            }
        }

        private void InitializeMainMemory()
        {

            _mainMemoryStorage = new int[1024];
            for (int i = 0; i < 1024; i++)
                _mainMemoryStorage[i]= ( i & 0xFF);
        }

        private void InitializeRegisters()
        {

            _registers = new int[32];
            _registers[0] = 0;
            for (int i = 1; i < 32; i++)
                _registers[i] = (0x100 +i ); 

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
            
        }

        private void CopyWriteToReadValues()
        {
            pipelineStage.ifid_read = pipelineStage.ifid_write;
            pipelineStage.idex_read = pipelineStage.idex_write;
            pipelineStage.exmem_read = pipelineStage.exmem_write;
            pipelineStage.memwb_read = pipelineStage.memwb_write;
        }

    }
}
