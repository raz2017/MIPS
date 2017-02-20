using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.PipelineStageClasses
{
    public class MemoryStage
    {
        public int RegWrite;
        public int MemToReg;

        public int LWDataValue;
        public int ALUResult;
        public int WriteRegNum;

        public MemoryStage()
        {
            RegWrite = MemToReg = 0;
            LWDataValue = ALUResult = WriteRegNum = 0;

        }

        public void SetWriteBackValues(ExecuteStage exmemRead)
        {
            this.RegWrite = exmemRead.RegWrite;
            this.MemToReg = exmemRead.MemToReg;

            this.ALUResult = exmemRead.ALUResult;
            this.WriteRegNum = exmemRead.WriteRegNum;
        }
    }
}
