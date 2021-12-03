using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace PokeWorld
{
	[HarmonyPatch(typeof(StockGenerator_Clothes))]
	[HarmonyPatch("HandlesThingDef")]
	public class StockGenerator_Clothes_HandlesThingDef_Patch
	{
		private static ISet<ThingDef> pwBelts;

		public static void Prepare()
		{
			// Cache matching defs during patching to avoid prohibitive performance overhead when a trader is on the map (#23)
			pwBelts = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.thingCategories != null && x.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("PW_Belts"))).ToHashSet();

		}

		public static void Postfix(ThingDef __0, ref bool __result)
		{
			if(__result == true && pwBelts.Contains(__0))
            {
				__result = false;
			}
		}
	
	}
}
