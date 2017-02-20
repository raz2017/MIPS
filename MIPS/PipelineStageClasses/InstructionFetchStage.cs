using System.Collections.Generic;

namespace MIPS.PipelineStageClasses
{
    public class InstructionFetchStage
    {
        private int _currentInstruction;
        public List<int> _instructions;

        public InstructionFetchStage(IInstructions instructions)
        {
            _currentInstruction = 0;
            _instructions = instructions.mipsInstructions;
        }

        public void FetchInstruction(int instructionIndex)
        {
            _currentInstruction = _instructions[instructionIndex];
        }

        public int CurrentInstruction
        {
            get
            {
                return _currentInstruction;
                
            }
        }
        
    }
}