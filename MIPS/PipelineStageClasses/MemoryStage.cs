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
    }
}
