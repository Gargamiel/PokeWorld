using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using HarmonyLib;
using RimWorld;

namespace PokeWorld
{
	[HarmonyPatch(typeof(PawnUtility))]
	[HarmonyPatch("Mated")]
	class PawnUtility_Mated_Patch
	{
		public static bool Prefix(Pawn __0, Pawn __1)
		{
			CompPokemon comp = __1.TryGetComp<CompPokemon>();
			if (comp == null || !comp.eggGroups.Contains(DefDatabase<EggGroupDef>.GetNamed("Ditto")))
			{
				return true;
			}
			CompDittoEggLayer compDittoEggLayer = __1.TryGetComp<CompDittoEggLayer>();
			if (compDittoEggLayer != null && !__0.health.hediffSet.HasHediff(HediffDefOf.Sterilized) && !__1.health.hediffSet.HasHediff(HediffDefOf.Sterilized))
			{
				compDittoEggLayer.Fertilize(__0);
			}
			return false;
		}
	}
}
