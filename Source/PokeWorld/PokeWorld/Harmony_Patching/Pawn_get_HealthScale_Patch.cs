using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using PokeWorld;

namespace PokeWorld
{
	[HarmonyPatch(typeof(Pawn))]
	[HarmonyPatch("get_HealthScale")]
	public class Pawn_get_HealthScale_Patch
	{		
		public static void Postfix(Pawn __instance, ref float __result)
		{
			if(__instance != null)
            {
				CompPokemon comp = __instance.TryGetComp<CompPokemon>();
				if (comp != null && comp.statTracker != null)
				{
					__result *= comp.statTracker.HealthScaleMult;
				}
			}			
		}
	}
}
