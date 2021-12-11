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
		public static void Postfix(ThingDef __0, ref bool __result)
		{
			if(__result == true && __0.thingCategories != null && __0.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("PW_Belts")))
            {
				__result = false;
			}
		}
	}
}
