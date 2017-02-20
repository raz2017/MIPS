#include <iostream>
#include <vector>

using namespace std;
//Nonmember functions for initialization
vector<int> initMainMemory(vector<int> & vec);
vector<int> initReg(vector<int> & reg);
vector<int> addInstructions();
//Decode
string rfunct(int x);
string opcode(int x);
bool r_format(int x);
int source_reg1(int x);
int source_reg2(int x);
int dest_reg(int x);
short offset(int x);
int func_code(int x);

struct IFID{
	int inst;
	IFID(){
		inst = 0;
	};
};

struct IDEX{

	int RegDst;
	int ALUSrc;
	int ALUOp;
	int MemRead;
	int MemWrite;
	int RegWrite;
	int MemToReg;

	int ReadReg1Value;
	int ReadReg2Value;
	int SEOffset;
	int WriteReg_20_16;
	int WriteReg_15_11;
	int Function;
	IDEX(){

		RegDst = ALUSrc = ALUOp = MemRead =
		MemWrite = RegWrite = MemToReg = 0;

		ReadReg1Value = ReadReg2Value = SEOffset =
		WriteReg_20_16 = WriteReg_15_11 = Function = 0;
	};
};


struct EXMEM{
	int MemRead;
	int MemWrite;
	int MemToReg;
	int RegWrite;

	int SWValue;
	int WriteRegNum;
	int ALUResult;
	EXMEM(){

		MemRead = MemWrite = MemToReg = RegWrite = 0;

		SWValue = WriteRegNum = ALUResult = 0;
	};
};



struct MEMWB{
	int RegWrite;
	int MemToReg;

	int LWDataValue;
	int ALUResult;
	int WriteRegNum;
	MEMWB(){

		RegWrite = MemToReg = 0;
		LWDataValue = ALUResult = WriteRegNum = 0;
	};
};



struct PipeLinedDatapath{

	vector<int> Main_Mem;
	vector<int> Regs;

	//Registers
	IFID ifid_w;
	IFID ifid_r;
	IDEX idex_w;
	IDEX idex_r;
	EXMEM exmem_w;
	EXMEM exmem_r;
	MEMWB memwb_w;
	MEMWB memwb_r;


	PipeLinedDatapath(){

		initMainMemory(Main_Mem);
		initReg(Regs);

	};

	void IF_stage(int inst);
	void ID_stage();
	void EX_stage();
	void MEM_stage();
	void WB_stage();
	void Print_out_everything();
	void Copy_write_to_read();

};

void PipeLinedDatapath::IF_stage(int inst){
	ifid_w.inst = inst;
};
void PipeLinedDatapath::ID_stage(){
	//No control signals here
	//We decode instruction
	//Figure out what the control bits are going to be for this particular instruction
	//Use Op Code for Control bits
	//Set Control

	int ReadRegister1 = source_reg1(ifid_r.inst);
	int ReadRegister2 = source_reg2(ifid_r.inst);
	int WriteRegister = dest_reg(ifid_r.inst);

	idex_w.ReadReg1Value = Regs[ReadRegister1];
	idex_w.ReadReg2Value = Regs[ReadRegister2];
	idex_w.WriteReg_20_16 = ReadRegister2;
	idex_w.WriteReg_15_11 = WriteRegister;
	idex_w.SEOffset = offset(ifid_r.inst);
	idex_w.Function = func_code(idex_w.SEOffset);

	if (r_format(ifid_r.inst) &&
		(rfunct(ifid_r.inst) == "sub" || rfunct(ifid_r.inst) =="add"))
	{
		idex_w.RegDst =1;
		idex_w.ALUOp=10;
		idex_w.ALUSrc = 0;
		idex_w.MemRead = 0;
		idex_w.MemWrite = 0;
		idex_w.RegWrite =1;
		idex_w.MemToReg = 0;
	}
	else if (opcode(ifid_r.inst) == "lb") {
		idex_w.RegDst =0;
		idex_w.ALUOp=0;
		idex_w.ALUSrc = 1;
		idex_w.MemRead = 1;
		idex_w.MemWrite = 0;
		idex_w.RegWrite =1;
		idex_w.MemToReg = 1;
	}
	else if (opcode(ifid_r.inst) =="sb"){
		idex_w.RegDst =0;
		idex_w.ALUOp=0;
		idex_w.ALUSrc = 1;
		idex_w.MemRead = 0;
		idex_w.MemWrite = 1;
		idex_w.RegWrite =0;
		idex_w.MemToReg = 0;
	}
	else {
		idex_w.RegDst =0;
		idex_w.ALUOp=0;
		idex_w.ALUSrc = 0;
		idex_w.MemRead = 0;
		idex_w.MemWrite = 0;
		idex_w.RegWrite = 0;
		idex_w.MemToReg = 0;
	};
};
void PipeLinedDatapath::EX_stage(){
	//Control signals

	//RegDest
	if (idex_r.RegDst == 1){
		exmem_w.WriteRegNum = idex_r.WriteReg_15_11;
	};
	if (idex_r.RegDst == 0){
		exmem_w.WriteRegNum = idex_r.WriteReg_20_16;
	};
	//ALUSrc

	int SecondALUOperand;
	if (idex_r.ALUSrc == 1){
		SecondALUOperand = idex_r.SEOffset;
	};
	if (idex_r.ALUSrc == 0){
		SecondALUOperand = idex_r.ReadReg2Value;
	};

	//ALUOp
	int ALUControlInput;
	if (idex_r.ALUOp == 0){
		ALUControlInput = 10;
	};
	if (idex_r.ALUOp == 10){
		if (idex_r.Function == 0x20){
			ALUControlInput =10;
		};
		if (idex_r.Function == 0x22){
			ALUControlInput = 110;
		};
	};
	//ALU
	if (ALUControlInput == 10){
		exmem_w.ALUResult = idex_r.ReadReg1Value + SecondALUOperand;
	}
	if (ALUControlInput == 110){
		exmem_w.ALUResult = idex_r.ReadReg1Value - SecondALUOperand;
	}


	//Store word value
	exmem_w.SWValue = idex_r.ReadReg2Value;

	exmem_w.MemRead = idex_r.MemRead;
	exmem_w.MemWrite = idex_r.MemWrite;
	exmem_w.MemToReg = idex_r.MemToReg;
	exmem_w.RegWrite = idex_r.RegWrite;


};

void PipeLinedDatapath::MEM_stage(){
	//Control Signals
	//R-Type should be NOP
	if (exmem_r.MemRead == 1){
		memwb_w.LWDataValue = Main_Mem[exmem_r.ALUResult];
	};

	if (exmem_r.MemWrite ==1){
		Main_Mem[exmem_r.ALUResult] = exmem_r.SWValue;
	};


	memwb_w.RegWrite = exmem_r.RegWrite;
	memwb_w.MemToReg = exmem_r.MemToReg;

	memwb_w.ALUResult = exmem_r.ALUResult;
	memwb_w.WriteRegNum = exmem_r.WriteRegNum;


};

void PipeLinedDatapath::WB_stage(){
	//SB shouldn't do anything here
	if (memwb_r.RegWrite ==1){
		if (memwb_r.MemToReg ==0){
			Regs[memwb_r.WriteRegNum] = memwb_r.ALUResult;
		};
		if (memwb_r.MemToReg == 1) {
			Regs[memwb_r.WriteRegNum] = memwb_r.LWDataValue;
		};

	};
};
void PipeLinedDatapath::Print_out_everything(){
	//Print out the Reg Values
	static int Cycle = 1;
	printf("%s %d\n\n", "Clock Cycle " ,Cycle);
	Cycle++;
	//Registers
	printf("%s\n", "32 Register Values:");
	for (int i =0; i< 32;i+=8 ) {
		printf("$%d=%x, $%d=%x, $%d=%x, $%d=%x, $%d=%x, $%d=%x, $%d=%x, $%d=%x\n",
			   i, Regs[i], i + 1, Regs[i + 1], i + 2, Regs[i + 2], i + 3, Regs[i + 3], i + 4, Regs[i + 4],
			   i + 5, Regs[i + 5], i + 6, Regs[i + 6], i + 7, Regs[i + 7]);
	}
	//IF/ID
	printf("\n%s\n", "IF/ID_Write (written to by the IF stage)");
	printf("%s = %x\n\n", "Inst", ifid_w.inst);

	printf("%s\n", "IF/ID_Read (read by the ID stage)");
	printf("%s = %x\n\n", "Inst", ifid_r.inst);

	//ID/EX
	//Control
	printf("%s\n\n", "ID/EX_Write (written to by the ID stage)");
	printf("%s\n", "Control:");
	printf("%s=%d, %s=%d, %s=%d\n",
		   "RegDst", idex_w.RegDst,"ALUSrc", idex_w.ALUSrc,
			"ALUOp", idex_w.ALUOp);
	printf("%s=%d, %s=%d, %s=%d %s=%d\n\n",
		   "MemRead", idex_w.MemRead,"MemWrite", idex_w.MemWrite,
		   "MemToReg", idex_w.MemToReg, "RegWrite", idex_w.RegWrite);
	//Data
	printf("%s\n", "Data Values:");
	printf("%s=%x, %s=%x, %s=%x\n", "ReadReg1Value", idex_w.ReadReg1Value,
		   "ReadReg2Value", idex_w.ReadReg2Value, "SEOffset", idex_w.SEOffset);
	printf("%s=%d, %s=%d, %s=%x\n\n", "WriteReg_20_16", idex_w.WriteReg_20_16,
		   "WriteReg_15_11", idex_w.WriteReg_15_11, "Function", idex_w.Function);

	//Control
	printf("%s\n\n", "ID/EX_Read (read by the EX stage)");
	printf("%s\n", "Control:");
	printf("%s=%d, %s=%d, %s=%d\n",
		   "RegDst", idex_r.RegDst,"ALUSrc", idex_r.ALUSrc,
		   "ALUOp", idex_r.ALUOp);
	printf("%s=%d, %s=%d, %s=%d, %s=%d\n\n",
		   "MemRead", idex_r.MemRead,"MemWrite", idex_r.MemWrite,
		   "MemToReg", idex_r.MemToReg, "RegWrite", idex_r.RegWrite);
	//Data
	printf("%s\n", "Data Values:");
	printf("%s=%x, %s=%x, %s=%x\n", "ReadReg1Value", idex_r.ReadReg1Value,
		   "ReadReg2Value", idex_r.ReadReg2Value, "SEOffset", idex_r.SEOffset);
	printf("%s=%d, %s=%d, %s=%x\n\n", "WriteReg_20_16", idex_r.WriteReg_20_16,
		   "WriteReg_15_11", idex_r.WriteReg_15_11, "Function", idex_r.Function);

	//EX/Mem
	//Control
	printf("%s\n\n", "EX/MEM_Write (written to by the EX stage)");
	printf("%s\n", "Control:");
	printf("%s=%d, %s=%d, %s=%d, %s=%d\n\n",
		   "MemRead", exmem_w.MemRead,"MemWrite", exmem_w.MemWrite,
		   "MemToReg", exmem_w.MemToReg, "RegWrite", exmem_w.RegWrite);
	//Data Values
	printf("%s\n", "Data Values:");
	printf("%s=%x, %s=%d, %s=%x\n\n", "SWValue", exmem_w.SWValue,
		   "WriteRegNum", exmem_w.WriteRegNum, "ALUResult", exmem_w.ALUResult);
	//Control
	printf("%s\n\n", "EX/MEM_Read (read by the MEM stage)");
	printf("%s\n", "Control:");
	printf("%s=%d, %s=%d, %s=%d, %s=%d\n\n",
		   "MemRead", exmem_r.MemRead,"MemWrite", exmem_r.MemWrite,
		   "MemToReg", exmem_r.MemToReg, "RegWrite", exmem_r.RegWrite);
	//Data Values
	printf("%s\n", "Data Values:");
	printf("%s=%x, %s=%d, %s=%x\n\n", "SWValue", exmem_r.SWValue,
		   "WriteRegNum", exmem_r.WriteRegNum, "ALUResult", exmem_r.ALUResult);

	//MEM/WB
	printf("%s\n\n", "MEM/WB_Write (written to by the MEM stage)");
	printf("%s\n", "Control:");
	printf("%s=%d, %s=%d\n\n", "MemToReg", memwb_w.MemToReg, "RegWrite", memwb_w.RegWrite);
	printf("%s\n", "Data Values:");
	printf("%s=%x, %s=%x, %s=%d\n\n", "LWDataValue", memwb_w.LWDataValue,
		   "ALUResult", memwb_w.ALUResult, "WriteRegNum", memwb_w.WriteRegNum);


	printf("%s\n\n", "MEM/WB_Read (read by the WB stage)");
	printf("%s\n", "Control:");
	printf("%s=%d, %s=%d\n\n", "MemToReg", memwb_r.MemToReg, "RegWrite", memwb_r.RegWrite);
	printf("%s\n", "Data Values:");
	printf("%s=%x, %s=%x, %s=%d\n\n", "LWDataValue", memwb_r.LWDataValue,
		   "ALUResult", memwb_r.ALUResult, "WriteRegNum", memwb_r.WriteRegNum);

};
void PipeLinedDatapath::Copy_write_to_read(){
	ifid_r = ifid_w;
	idex_r = idex_w;
	exmem_r = exmem_w;
	memwb_r = memwb_w;
};



int main() {

	vector<int> instructions = addInstructions();
	PipeLinedDatapath pipe;

	for (int i =0; i< instructions.size();i++){

		pipe.IF_stage(instructions[i]);
		pipe.ID_stage();
		pipe.EX_stage();
		pipe.MEM_stage();
		pipe.WB_stage();
		pipe.Print_out_everything();
		pipe.Copy_write_to_read();
	}


};

vector<int> initMainMemory(vector<int> & vec){
	vec.resize(1024);
	for (int i = 0; i < 1024; i++){ vec[i] = i & 0x0FF; };
	return vec;
};
vector<int> initReg(vector<int> & reg){
	reg.resize(32);
	reg[0]= 0;
	for (int i = 1 ; i< 32; i++){ reg[i] = 0x100 + i;};
	return reg;
};
vector<int> addInstructions(){
	vector<int> inst;
	inst.push_back(0xa1020000);
	inst.push_back(0x810AFFFC);
	inst.push_back(0x00831820);
	inst.push_back(0x01263820);
	inst.push_back(0x01224820);
	inst.push_back(0x81180000);
	inst.push_back(0x81510010);
	inst.push_back(0x00624022);
	inst.push_back(0x00000000);
	inst.push_back(0x00000000);
	inst.push_back(0x00000000);
	inst.push_back(0x00000000);
	return inst;
};

string rfunct(int x){
	switch(x & 0x0000003f){
		case 0x22:
			return "sub";
		case 0x20:
			return "add";
		case 0x00:
			return "nop";
		default:
			return "NotDef";
	};
};

string opcode(int x){
	switch( (x & 0xFC000000) >>26 ){
		case 0x20:
			return "lb";
		case 0x28:
			return "sb";
		default:
			return "NotDef";
	};
};

bool r_format(int x){
	return (x & 0xFC000000) == 0? true:false;
};

int source_reg1(int x){
	return (x & 0x03e00000)>>21;
};

int source_reg2(int x){
	return (x & 0x001f0000) >>16;
};

int dest_reg(int x){
	return (x & 0x0000f800) >> 11;
};

short offset(int x){
	return x & 0x0000ffff;
};

int func_code(int x){
	return x & 0x0000003f;
};