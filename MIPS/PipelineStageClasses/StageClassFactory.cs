using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.PipelineStageClasses;

namespace MIPS
{
    public static class StageClassFactory
    {
        public static InstructionFetchStage getInstructionFetchClass()
        {
            return new InstructionFetchStage();
        }

        public static InstructionDecodeStage getInstructionDecodeClass()
        {
            return new InstructionDecodeStage();
        }

        public static ExecuteStage getExecuteStageClass()
        {
            return new ExecuteStage();
        }

        public static MemoryStage getMemoryStageClass()
        {
            return new MemoryStage();
        }
    }
}
