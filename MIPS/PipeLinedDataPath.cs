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

        private InstructionFetchStage ifid_write;
        private InstructionFetchStage ifid_read;
        private InstructionDecodeStage idex_write;
        private InstructionDecodeStage idex_read;
        private ExecuteStage exmem_write;
        private ExecuteStage exmem_read;
        private MemoryStage memwb_write;
        private MemoryStage memwb_read;

        private readonly Instructions _instructions;

        public PipeLinedDataPath(Instructions instructions)
        {
        
            ifid_write = StageClassFactory.getInstructionFetchClass(instructions);
            ifid_read = StageClassFactory.getInstructionFetchClass(instructions);

            idex_write = StageClassFactory.getInstructionDecodeClass();
            idex_read = StageClassFactory.getInstructionDecodeClass();

            exmem_write = StageClassFactory.getExecuteStageClass();
            exmem_read = StageClassFactory.getExecuteStageClass();

            memwb_write = StageClassFactory.getMemoryStageClass();
            memwb_read = StageClassFactory.getMemoryStageClass();

            InitializeMainMemory();
            InitializeRegisters();

            _instructions = instructions;
            _instructionIndex = 0;
        }

        public void IF_FetchInstructions()
        {
            ifid_write.FetchInstruction(_instructionIndex);
            _instructionIndex++;

        }

        public void ID_DecodeInstructions()
        {

            idex_write.SetRegisterValues(ifid_read.CurrentInstruction,_registers );

            idex_write.SetExecutionPath(ifid_read.CurrentInstruction);

        }

        public void EX_ExecuteInstructions()
        {
            if (idex_read._regDestination == 1)
            {
                exmem_write.WriteRegNum = idex_read.WriteReg_15_11;
            }
            if (idex_read._regDestination == 0)
            {
                exmem_write.WriteRegNum = idex_read.WriteReg_20_16;
            }

            int SecondALUOperand = 0;
            if (idex_read._ALUSrc == 1)
            {
                SecondALUOperand = idex_read.SEOffset;
            }

            if (idex_read._ALUSrc == 0)
            {
                SecondALUOperand = idex_read.ReadReg2Value;
            }

            int aluControlInput = 0;
            if (idex_read._ALUOp == 0)
            {
                aluControlInput = 10;
            }

            if (idex_read._ALUOp == 10)
            {
                if (idex_read.Function == 0x20)
                {
                    aluControlInput = 10;
                }
                if (idex_read.Function == 0x22)
                {
                    aluControlInput = 110;
                }
            }

            if (aluControlInput == 10)
            {
                exmem_write.ALUResult = idex_read.ReadReg1Value + SecondALUOperand;
            }
            if (aluControlInput == 110)
            {
                exmem_write.ALUResult = idex_read.ReadReg1Value - SecondALUOperand;
            }

            exmem_write.SWvalue = idex_read.ReadReg2Value;

            exmem_write.MemRead = idex_read._MemRead;
            exmem_write.MemWrite = idex_read._MemWrite;
            exmem_write.MemToReg = idex_read._MemToReg;
            exmem_write.RegWrite = idex_read._RegWrite;
        }

        public void MEM_LoadValueFromMemory()
        {
            if (exmem_write.MemRead == 1)
            {
                memwb_write.LWDataValue = _mainMemoryStorage[exmem_read.ALUResult];
            }

            if (exmem_read.MemWrite == 1)
            {
                _mainMemoryStorage[exmem_read.ALUResult] = exmem_read.SWvalue;
            }

            memwb_write.RegWrite = exmem_read.RegWrite;
            memwb_write.MemToReg = exmem_read.MemToReg;

            memwb_write.ALUResult = exmem_read.ALUResult;
            memwb_write.WriteRegNum = exmem_read.WriteRegNum;

        }

        public void WB_StoreValuesToRegisters()
        {
            if (memwb_read.RegWrite == 1)
            {
                if (memwb_read.MemToReg == 0)
                {
                    _registers[memwb_read.WriteRegNum] = memwb_read.ALUResult;
                }
                if (memwb_read.MemToReg == 1)
                {
                    _registers[memwb_read.WriteRegNum] = memwb_read.LWDataValue;
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
            ifid_read = ifid_write;
            idex_read = idex_write;
            exmem_read = exmem_write;
            memwb_read = memwb_write;
        }

    }
}
