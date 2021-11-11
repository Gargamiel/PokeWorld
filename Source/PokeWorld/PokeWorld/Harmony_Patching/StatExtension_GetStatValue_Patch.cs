using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;

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
			if (comp != null)
			{
				if(__1 == StatDefOf.MeleeHitChance)
                {
					__result += (comp.levelTracker.level / 100f);
				}
				else if (__1 == StatDefOf.ArmorRating_Sharp)
				{
					__result = Mathf.Clamp((1 / 4.0f * comp.statTracker.defenseStat + 3 / 4.0f * comp.statTracker.defenseSpStat) / 120, 0, 1.5f);
				}
				else if (__1 == StatDefOf.ArmorRating_Blunt)
				{
					__result = Mathf.Clamp((3 / 4.0f * comp.statTracker.defenseStat + 1 / 4.0f * comp.statTracker.defenseSpStat) / 120, 0, 1.5f);
				}
				else if (__1 == StatDefOf.ArmorRating_Heat)
				{
                    if (comp.types.Contains(DefDatabase<TypeDef>.GetNamed("Fire")))
                    {
						__result = 1.0f;
					}
                    else
                    {
						__result = Mathf.Clamp(comp.statTracker.defenseSpStat / 200.0f, 0, 1.0f);
					}					
				}
			}			
		}
	}
}
