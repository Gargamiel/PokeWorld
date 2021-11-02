using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;

namespace PokeWorld
{
	[HarmonyPatch(typeof(StatExtension))]
	[HarmonyPatch("GetStatValue")]
	class StatExtension_GetStatValue_Patch
	{
		public static void Postfix(Thing __0, StatDef __1, ref float __result)
		{
			Pawn pawn = __0 as Pawn;
			CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			if (comp != null && __1.defName == "MeleeHitChance")
			{
				__result += (comp.levelTracker.level /100f);
			}			
		}
	}
}
