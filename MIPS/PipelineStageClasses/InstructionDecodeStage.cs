using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.PipelineStageClasses
{
    public class InstructionDecodeStage
    {
        public int _regDestination;
        public int _ALUSrc;
        public int _ALUOp;
        public int _MemRead;
        public int _MemWrite;
        public int _RegWrite;
        public int _MemToReg;

        public int ReadReg1Value;
        public int ReadReg2Value;
        public int SEOffset;
        public int WriteReg_20_16;
        public int WriteReg_15_11;

        public int Function;

        public InstructionDecodeStage()
        {
            _regDestination = _ALUSrc = _ALUOp = _MemRead =
                _MemWrite = _RegWrite = _MemToReg = 0;

            ReadReg1Value = ReadReg2Value = SEOffset =
                WriteReg_20_16 = WriteReg_15_11 = Function = 0;
        }


        public void SetRegisterValues(int currentInstruction, int[] registers)
        {
            int readRegister1 = InstructionDecoder.source_reg1(currentInstruction);
            int readRegister2 = InstructionDecoder.source_reg2(currentInstruction);
            int writeRegister = InstructionDecoder.dest_reg(currentInstruction);


            ReadReg1Value = registers[readRegister1];
            ReadReg2Value = registers[readRegister2];
            WriteReg_20_16 = readRegister2;
            WriteReg_15_11 = writeRegister;
            SEOffset = InstructionDecoder.offset(currentInstruction);
            Function = InstructionDecoder.func_code(SEOffset);

        }

        public void SetExecutionPath(int currentinstruction)
        {
            if ((InstructionDecoder.is_r_format(currentinstruction)) &&
                ((InstructionDecoder.rfunct(currentinstruction) == InstructionType.Subtract) ||
                 InstructionDecoder.rfunct(currentinstruction) == InstructionType.Add))
            {
                _regDestination = 1;
                _ALUOp = 10;
                _ALUSrc = 0;
                _MemRead = 0;
                _MemWrite = 0;
                _RegWrite = 1;
                _MemToReg = 0;
            }

            else if (InstructionDecoder.getOPcode(currentinstruction) == InstructionType.LoadByte)
            {
                _regDestination = 0;
                _ALUOp = 0;
                _ALUSrc = 1;
                _MemRead = 1;
                _MemWrite = 0;
                _RegWrite = 1;
                _MemToReg = 1;
            }

            else if (InstructionDecoder.getOPcode(currentinstruction) == InstructionType.StoreByte)
            {
                _regDestination = 0;
                _ALUOp = 0;
                _ALUSrc = 1;
                _MemRead = 0;
                _MemWrite = 1;
                _RegWrite = 0;
                _MemToReg = 0;
            }

            else
            {
                _regDestination = 0;
                _ALUOp = 0;
                _ALUSrc = 0;
                _MemRead = 0;
                _MemWrite = 0;
                _RegWrite = 0;
                _MemToReg = 0;
            }


        }

        public int GetALUControlInput()
        {
            int aluControlInput = 0;

            //I-type instruction or R-Type Add instruction
            if (this._ALUOp == 0 || (this._ALUOp == 10 && this.Function == 0x20))
                aluControlInput = 10; //ADD

            //R-Type Subtract instruction
            if (this._ALUOp == 10 && this.Function == 0x22)
                aluControlInput = 110; //SUBTRACT
 
            return aluControlInput;
        }
    }
}
