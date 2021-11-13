using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace PokeWorld
{
    [HarmonyPatch(typeof(ThingSetMaker_MapGen_AncientPodContents))]
    [HarmonyPatch("GenerateScarabs")]
    public class ThingSetMaker_MapGen_AncientPodContents_GenerateScarabs_Patch
	{
        public static bool Prefix(ref List<Thing> __result)
        {
            if (PokeWorldSettings.OkforPokemon() && PokeWorldSettings.allowGen2)
            {
				__result = GenerateUnown();
				return false;
			}
			return true;
		}
		private static List<Thing> GenerateUnown()
		{
			List<Thing> list = new List<Thing>();
			int num = Rand.Range(2, 4);
			for (int i = 0; i < num; i++)
			{
				Pawn pawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("PW_Unown"));
				list.Add(pawn);
			}
			return list;
		}
	}
}
