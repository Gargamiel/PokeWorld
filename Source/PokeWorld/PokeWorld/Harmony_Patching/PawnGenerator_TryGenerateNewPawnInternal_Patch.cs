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

	[HarmonyPatch(typeof(PawnGenerator))]
	[HarmonyPatch("TryGenerateNewPawnInternal")]
	public class PawnGenerator_TryGenerateNewPawnInternal_Patch
	{
		public static void Prefix(ref PawnGenerationRequest __0)
		{
            if (__0.KindDef != null && __0.KindDef.race.HasComp(typeof(CompPokemon)) && __0.KindDef.RaceProps.hasGenders)
            {
                float femaleRatio = __0.KindDef.race.GetCompProperties<CompProperties_Pokemon>().femaleRatio;
                if (Rand.Value <= femaleRatio)
                {
                    __0.FixedGender= Gender.Female;
                }
                else
                {
                    __0.FixedGender = Gender.Male;
                }
            }
		}
	}
	
}
