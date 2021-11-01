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

	[HarmonyPatch(typeof(Pawn))]
	[HarmonyPatch("TryGetAttackVerb")]
	class Pawn_TryGetAttackVerb_Patch
	{
		public static bool Prefix(Pawn __instance, ref Verb __result)
		{
			CompPokemon comp = __instance.TryGetComp<CompPokemon>();
			if (comp == null || comp.moveTracker == null)
			{
				return true;
			}
			if(PokemonAttackGizmoUtility.CanUseAnyRangedVerb(__instance))
			{
				__result = PokemonAttackGizmoUtility.GetAnyRangedVerb(__instance);
				return false;
			}
			return true;
		}
	}

}
