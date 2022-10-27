using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSML.Patcher
{
	public class AssemblyHelper
	{
		public static Instruction LoadAssemblyInstruction(ILProcessor il, MethodReference assemblyLoadFunc, string assemblyPath)
		{
			Instruction loadStringInstr = il.Create(OpCodes.Ldstr, assemblyPath);
			il.Body.Instructions.Add(loadStringInstr);

			Instruction loadAssemblyInstr = il.Create(OpCodes.Call, assemblyLoadFunc);
			il.InsertAfter(loadStringInstr, loadAssemblyInstr);

			il.InsertAfter(loadAssemblyInstr, il.Create(OpCodes.Pop));
			return loadAssemblyInstr;
		}
	}
}
