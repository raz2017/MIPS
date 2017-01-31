using System;
using System.Collections.Generic;
using System.Linq;
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


    }
}
