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
        private List<int> _mainMemoryStorage;
        private List<int> _registers;

        private InstructionFetchStage ifid_write;
        private InstructionFetchStage ifid_read;
        private InstructionDecodeStage idex_write;
        private InstructionDecodeStage idex_read;
        private ExecuteStage exmem_write;
        private ExecuteStage exmem_read;
        private MemoryStage memwb_write;
        private MemoryStage memwb_read;

        private readonly List<int> _instructions;

        public PipeLinedDataPath(Instructions instructions)
        {
            _mainMemoryStorage = new List<int>();
            _registers = new List<int>();
            InitializeMainMemory();
            InitializeRegisters();

            _instructions = instructions.mipsInstructions;
        }

        public void IF_FetchInstructions(int instruction)
        {
            ifid_write._instruction = instruction;

        }

        public void ID_DecodeInstructions()
        {
            int ReadRegister1 = DecodeInstruction.source_reg1(ifid_read._instruction);
            int ReadRegister2 = DecodeInstruction.source_reg2(ifid_read._instruction);
            int WriteRegister = DecodeInstruction.dest_reg(ifid_read._instruction);

            idex_write.ReadReg1Value = _registers[ReadRegister1];
            idex_write.ReadReg2Value = _registers[ReadRegister2];
            idex_write.WriteReg_20_16 = ReadRegister2;
            idex_write.WriteReg_15_11 = WriteRegister;
            idex_write.SEOffset = DecodeInstruction.offset(ifid_read._instruction);
            idex_write.Function = DecodeInstruction.func_code(idex_write.SEOffset);

            if ((DecodeInstruction.is_r_format(ifid_read._instruction)) && 
                   ( (DecodeInstruction.rfunct(ifid_read._instruction) == "sub") ||
                    DecodeInstruction.rfunct(ifid_read._instruction) == "add" ))
            {
                idex_write._regDestination = 1;
                idex_write._ALUOp = 10;
                idex_write._ALUSrc = 0;
                idex_write._MemRead = 0;
                idex_write._MemWrite = 0;
                idex_write._RegWrite = 1;
                idex_write._MemToReg = 0;
            }

            else if (DecodeInstruction.getOPcode(ifid_read._instruction) == "lb")
            {
                idex_write._regDestination = 0;
                idex_write._ALUOp = 0;
                idex_write._ALUSrc = 1;
                idex_write._MemRead = 1;
                idex_write._MemWrite = 0;
                idex_write._RegWrite = 1;
                idex_write._MemToReg = 1;
            }

            else if (DecodeInstruction.getOPcode(ifid_read._instruction) == "sb")
            {
                idex_write._regDestination = 0;
                idex_write._ALUOp = 0;
                idex_write._ALUSrc = 1;
                idex_write._MemRead = 0;
                idex_write._MemWrite = 1;
                idex_write._RegWrite = 0;
                idex_write._MemToReg = 0;
            }

            else
            {
                idex_write._regDestination = 0;
                idex_write._ALUOp = 0;
                idex_write._ALUSrc = 0;
                idex_write._MemRead = 0;
                idex_write._MemWrite = 0;
                idex_write._RegWrite = 0;
                idex_write._MemToReg = 0;
            }

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

            int ALUControlInput = 0;
            if (idex_read._ALUOp == 0)
            {
                ALUControlInput = 10;
            }

            if (idex_read._ALUOp == 10)
            {
                if (idex_read.Function == 0x20)
                {
                    ALUControlInput = 10;
                }
                if (idex_read.Function == 0x22)
                {
                    ALUControlInput = 110;
                }
            }

            if (ALUControlInput == 10)
            {
                exmem_write.ALUResult = idex_read.ReadReg1Value + SecondALUOperand;
            }
            if (ALUControlInput == 110)
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
 //           _mainMemoryStorage.Capacity = 1024;

            for (int i = 0; i < 1024; i++)
                _mainMemoryStorage.Add ( i & 0xFF);
        }

        private void InitializeRegisters()
        {
            //_registers= 32;

            _registers.Add(0);
            for (int i = 1; i < 32; i++)
                _registers.Add(0x100 +i ); 

        }

        public void ProcessInstructions()
        {
            for (int i = 0; i < _instructions.Count(); i++)
            {
                IF_FetchInstructions(_instructions[i]);
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
