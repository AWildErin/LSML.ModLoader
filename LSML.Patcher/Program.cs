using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
using LSML.ModLoader;

namespace LSML.Patcher
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				TryPatch();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex}");
			}
		}

		public static bool TryPatch()
		{
			string asm = @"E:\Repositories\landlordssuper\LSML\Deps\Assembly-CSharp.dll";
			ModuleDefinition module = ModuleDefinition.ReadModule(
				asm,
				new ReaderParameters() { ReadWrite = false, InMemory = true, ReadingMode = ReadingMode.Immediate });

			// Sting is a class that gets fired upon start up, making it an ideal candidate for us to hook into.
			var type = module.Types.Where(x => x.Name == nameof(Sting)).First();
			if (type == null)
				return false;

			// Create an awake method so we can fire stuff as soon as it's made.
			{
				MethodDefinition awake = new MethodDefinition(
					"Awake",
					Mono.Cecil.MethodAttributes.Private,
					module.TypeSystem.Void);
				ILProcessor ilp = awake.Body.GetILProcessor();

				MethodReference asmLoadFunc = module.ImportReference(typeof(Assembly).GetMethod("LoadFrom", new[] { typeof(string) }));
				string modLoaderAsmPath = Path.Combine("LSML_Data\\Managed\\", "LSML.ModLoader.dll");
				AssemblyHelper.LoadAssemblyInstruction(ilp, asmLoadFunc, modLoaderAsmPath);

				awake.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
				type.Methods.Insert(0, awake);
			}

			// VHSGo is called once, and fully plays even when we skip.
			var method = type.Methods.Where(x => x.Name == "VHSGo").First();
			if (method == null)
				return false;

			{
				ILProcessor ilp = method.Body.GetILProcessor();

				MethodReference cbs = module.ImportReference(typeof(ModLoader.ModLoader).GetMethod("LoadModLoader"));
				method.Body.Instructions.Insert(0, ilp.Create(OpCodes.Call, cbs));
			}

			module.Write("D:\\Games\\Games\\Landlord's Super\\LandlordsSuper_Data\\Managed\\Assembly-CSharp.dll");
			module.Dispose();

			return true;
		}
	}
}
