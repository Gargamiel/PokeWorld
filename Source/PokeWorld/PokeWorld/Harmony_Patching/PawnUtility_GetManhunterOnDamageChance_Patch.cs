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
	[HarmonyPatch(typeof(PawnUtility), "GetManhunterOnDamageChance", new Type[] { typeof(Pawn), typeof(float), typeof(Thing) })]
	class PawnUtility_GetManhunterOnDamageChance_Patch
	{
		public static void Postfix(Pawn __0, float __1, Thing __2, ref float __result)
		{
			if(__2 != null)
            {
				Pawn instigator = __2 as Pawn;
				CompPokemon instigatorComp = instigator.TryGetComp<CompPokemon>();
				if(instigatorComp != null)
                {
					CompPokemon targetComp = __0.TryGetComp<CompPokemon>();
					if(targetComp != null)
                    {

						__result *= GenMath.LerpDoubleClamped(-10f, 10f, 1f, 3f, targetComp.levelTracker.level - instigatorComp.levelTracker.level);
					}
                }
			}		
		}
	}
}
