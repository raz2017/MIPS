using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.PipelineStageClasses
{
    public class ExecuteStage
    {  
        public int MemRead;
        public int MemWrite;
        public int MemToReg;
        public int RegWrite;

        public int SWvalue;
        public int WriteRegNum;
        public int ALUResult;

        public ExecuteStage()
        {
            MemRead = MemWrite = MemToReg = RegWrite = 0;
            SWvalue = WriteRegNum = ALUResult = 0;
        }

        public void SetWriteRegisterNumber(InstructionDecodeStage idexRead)
        {
 
            //R-Type Instruction -- Writing to register in 11-15 bits
            WriteRegNum = idexRead._regDestination == 1 ? idexRead.WriteReg_15_11 : idexRead.WriteReg_20_16;
 
        }

        public void SetALUResult(InstructionDecodeStage idexRead, int aluOperand, int aluControlInput)
        {
            if (aluControlInput == 10)
                this.ALUResult = idexRead.ReadReg1Value + aluOperand;
            if (aluControlInput == 110)
                this.ALUResult = idexRead.ReadReg1Value - aluOperand;
        }

        public void SetMemoryControlsBits(InstructionDecodeStage idexRead)
        {
            this.SWvalue = idexRead.ReadReg2Value;
            this.MemRead = idexRead._MemRead;
            this.MemWrite = idexRead._MemWrite;
            this.MemToReg = idexRead._MemToReg;
            this.RegWrite = idexRead._RegWrite;
        }

    }
}
