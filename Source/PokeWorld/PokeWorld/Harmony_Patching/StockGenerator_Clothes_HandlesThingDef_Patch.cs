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
		public static void Postfix(ThingDef __0, bool __result)
		{
			if(__result == true)
            {
				if (DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.thingCategories != null && x.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("PW_Belts"))).Contains(__0))
				{
					__result = false;
				}
			}
		}
	
	}
}
