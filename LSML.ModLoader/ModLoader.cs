using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.CrashReportHandler;

namespace LSML.ModLoader
{
	public class ModLoader
	{
		public static void LoadModLoader()
		{
			Debug.Log("====== LSML LOADING ======");

			CrashReportHandler.enableCaptureExceptions = false;

			Harmony harmony = new Harmony("awilderin.lsml");
			harmony.PatchAll();

			Debug.Log("====== LSML LOADED ======");
		}
	}

	[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OpenStreamingAssetsPath))]
	public class Testing
	{
		[HarmonyPrefix]
		public static void Prefix()
		{
			Debug.Log("We hooked into a method");
		}
	}
}
