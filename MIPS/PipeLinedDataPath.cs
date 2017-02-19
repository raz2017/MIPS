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

        public void IF_FetchInstructions()
        {
            pipelineStage.ifid_write.FetchInstruction(_instructionIndex);
            _instructionIndex++;

        }

        public void ID_DecodeInstructions()
        {

            pipelineStage.idex_write.SetRegisterValues(pipelineStage.ifid_read.CurrentInstruction,_registers );
            pipelineStage.idex_write.SetExecutionPath(pipelineStage.ifid_read.CurrentInstruction);

        }

        public void EX_ExecuteInstructions()
        {
            if (pipelineStage.idex_read._regDestination == 1)
            {
                pipelineStage.exmem_write.WriteRegNum = pipelineStage.idex_read.WriteReg_15_11;
            }
            if (pipelineStage.idex_read._regDestination == 0)
            {
                pipelineStage.exmem_write.WriteRegNum = pipelineStage.idex_read.WriteReg_20_16;
            }

            int SecondALUOperand = 0;
            if (pipelineStage.idex_read._ALUSrc == 1)
            {
                SecondALUOperand = pipelineStage.idex_read.SEOffset;
            }

            if (pipelineStage.idex_read._ALUSrc == 0)
            {
                SecondALUOperand = pipelineStage.idex_read.ReadReg2Value;
            }

            int aluControlInput = 0;
            if (pipelineStage.idex_read._ALUOp == 0)
            {
                aluControlInput = 10;
            }

            if (pipelineStage.idex_read._ALUOp == 10)
            {
                if (pipelineStage.idex_read.Function == 0x20)
                {
                    aluControlInput = 10;
                }
                if (pipelineStage.idex_read.Function == 0x22)
                {
                    aluControlInput = 110;
                }
            }

            if (aluControlInput == 10)
            {
                pipelineStage.exmem_write.ALUResult = pipelineStage.idex_read.ReadReg1Value + SecondALUOperand;
            }
            if (aluControlInput == 110)
            {
                pipelineStage.exmem_write.ALUResult = pipelineStage.idex_read.ReadReg1Value - SecondALUOperand;
            }

            pipelineStage.exmem_write.SWvalue = pipelineStage.idex_read.ReadReg2Value;

            pipelineStage.exmem_write.MemRead = pipelineStage.idex_read._MemRead;
            pipelineStage.exmem_write.MemWrite = pipelineStage.idex_read._MemWrite;
            pipelineStage.exmem_write.MemToReg = pipelineStage.idex_read._MemToReg;
            pipelineStage.exmem_write.RegWrite = pipelineStage.idex_read._RegWrite;
        }

        public void MEM_LoadValueFromMemory()
        {
            if (pipelineStage.exmem_write.MemRead == 1)
            {
                pipelineStage.memwb_write.LWDataValue = _mainMemoryStorage[pipelineStage.exmem_read.ALUResult];
            }

            if (pipelineStage.exmem_read.MemWrite == 1)
            {
                _mainMemoryStorage[pipelineStage.exmem_read.ALUResult] = pipelineStage.exmem_read.SWvalue;
            }

            pipelineStage.memwb_write.RegWrite = pipelineStage.exmem_read.RegWrite;
            pipelineStage.memwb_write.MemToReg = pipelineStage.exmem_read.MemToReg;

            pipelineStage.memwb_write.ALUResult = pipelineStage.exmem_read.ALUResult;
            pipelineStage.memwb_write.WriteRegNum = pipelineStage.exmem_read.WriteRegNum;

        }

        public void WB_StoreValuesToRegisters()
        {
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
