using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.PipelineStageClasses;

namespace MIPS
{
    struct PipeLineStagesContainer
    {
        public InstructionFetchStage ifid_write;
        public InstructionFetchStage ifid_read;
        public InstructionDecodeStage idex_write;
        public InstructionDecodeStage idex_read;
        public ExecuteStage exmem_write;
        public ExecuteStage exmem_read;
        public MemoryStage memwb_write;
        public MemoryStage memwb_read;

        public PipeLineStagesContainer(Instructions instructions)
        {

            ifid_write = StageClassFactory.getInstructionFetchClass(instructions);
            ifid_read = StageClassFactory.getInstructionFetchClass(instructions);

            idex_write = StageClassFactory.getInstructionDecodeClass();
            idex_read = StageClassFactory.getInstructionDecodeClass();

            exmem_write = StageClassFactory.getExecuteStageClass();
            exmem_read = StageClassFactory.getExecuteStageClass();

            memwb_write = StageClassFactory.getMemoryStageClass();
            memwb_read = StageClassFactory.getMemoryStageClass();

        }
    }
}
