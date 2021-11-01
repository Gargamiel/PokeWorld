using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;
using HarmonyLib;

namespace PokeWorld
{
	[HarmonyPatch(typeof(SymbolResolver_Interior_AncientTemple))]
	[HarmonyPatch("Resolve")]
	public class SymbolResolver_Interior_AncientTemple_Resolve_Patch
	{
		public static void Postfix(ResolveParams __0)
		{
			Map map = BaseGen.globalSettings.map;
			int randInt = Rand.RangeInclusive(4, 7);
			for(int i = 0; i < randInt; i++)
            {
				ResolveParams resolveParams = __0;
				resolveParams.singlePawnKindDef = DefDatabase<PawnKindDef>.GetNamed("PW_Unown");
				BaseGen.symbolStack.Push("pawn", resolveParams);
			}
			int randInt2 = Rand.RangeInclusive(1, 3);
			for (int i = 0; i < randInt2; i++)
			{
				ResolveParams resolveParams2 = __0;
				resolveParams2.singlePawnKindDef = DefDatabase<PawnKindDef>.GetNamed("PW_Bronzor");
				BaseGen.symbolStack.Push("pawn", resolveParams2);
			}
			if(Rand.Value < 0.3)
            {
				ResolveParams resolveParams3 = __0;
				resolveParams3.singlePawnKindDef = DefDatabase<PawnKindDef>.GetNamed("PW_Bronzong");
				BaseGen.symbolStack.Push("pawn", resolveParams3);
			}
		}
	}
}
