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


    }
}
