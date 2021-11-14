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
            if (PokeWorldSettings.OkforPokemon())
            {
				__result = GenerateUnownAndBronzor();
				if(__result.Count > 0)
                {
					return false;
				}
                else
                {
					return true;
                }
			}
			return true;
		}
		private static List<Thing> GenerateUnownAndBronzor()
		{
			List<Thing> list = new List<Thing>();
			if (PokeWorldSettings.allowGen2)
            {
				int num = Rand.Range(3, 5);
				for (int i = 0; i < num; i++)
				{
					Pawn unown = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("PW_Unown"));
					list.Add(unown);
				}
			}
			if (PokeWorldSettings.allowGen4)
            {
				Pawn bronzor = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.GetNamed("PW_Bronzor"));
				list.Add(bronzor);
			}
			return list;
		}
	}
}
