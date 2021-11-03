using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace PokeWorld
{
    class FossilsEvoStoneDropper_Patch
    {
		[HarmonyPatch(typeof(Thing))]
		[HarmonyPatch("ButcherProducts")]
		public class Thing_ButcherProducts_Patch
		{
			public static void Postfix(Thing __instance, Pawn __0, ref IEnumerable<Thing> __result)
			{
				CompPokFossilsEvoStoneDropper comp = __instance.TryGetComp<CompPokFossilsEvoStoneDropper>();
				if (comp != null)
				{
					ThingDef def = FossilEvoStoneDropperUtility.TryGetItem(comp.stoneDropRate, comp.fossilDropRate);
					if (def != null)
					{
						Thing thing = ThingMaker.MakeThing(def);
						thing.stackCount = 1;
						List<Thing> list = __result.ToList();
						list.Add(thing);
						__result = list.AsEnumerable();
						if (__0 != null && __0.Faction == Faction.OfPlayer)
						{
							Messages.Message("PW_FoundFossilOrEvoStone".Translate(__0.LabelShortCap, thing.Label), thing, MessageTypeDefOf.PositiveEvent);
						}
					}					
				}
			}
		}
	}

	[HarmonyPatch(typeof(Mineable))]
	[HarmonyPatch("TrySpawnYield")]
	public class Mineable_TrySpawnYield_Patch
	{
		public static void Postfix(Mineable __instance, Map __0, Pawn __3)
		{
			CompPokFossilsEvoStoneDropper comp = __instance.TryGetComp<CompPokFossilsEvoStoneDropper>();
			if (comp != null)
			{
				ThingDef def = FossilEvoStoneDropperUtility.TryGetItem(comp.stoneDropRate, comp.fossilDropRate);
				if (def != null)
                {
					Thing thing = ThingMaker.MakeThing(def);
					thing.stackCount = 1;
					GenPlace.TryPlaceThing(thing, __instance.Position, __0, ThingPlaceMode.Near, ForbidIfNecessary);
					if (__3 != null && __3.Faction == Faction.OfPlayer)
                    {
						Messages.Message("PW_FoundFossilOrEvoStone".Translate(__3.LabelShortCap, thing.Label), thing, MessageTypeDefOf.PositiveEvent);
					}
				}
			}
			void ForbidIfNecessary(Thing thing, int count)
			{
				if ((__3 == null || !__3.IsColonist) && thing.def.EverHaulable && !thing.def.designateHaulable)
				{
					thing.SetForbidden(value: true, warnOnFail: false);
				}
			}
		}
	}

	[HarmonyPatch(typeof(CompDeepDrill))]
	[HarmonyPatch("TryProducePortion")]
	public class CompDeepDrill_TryProducePortion_Patch
	{
		public static void Postfix(CompDeepDrill __instance)
		{
			CompPokFossilsEvoStoneDropper comp = __instance.parent.TryGetComp<CompPokFossilsEvoStoneDropper>();
			if (comp != null)
			{
				ThingDef def = FossilEvoStoneDropperUtility.TryGetItem(comp.stoneDropRate, comp.fossilDropRate);
				if (def != null)
				{
					Thing thing = ThingMaker.MakeThing(def);
					thing.stackCount = 1;
					GenPlace.TryPlaceThing(thing, __instance.parent.Position, __instance.parent.Map, ThingPlaceMode.Near);
					Building building = __instance.parent as Building;
					Pawn user = building.InteractionCell.GetFirstPawn(building.Map);
					if (user != null && user.Faction == Faction.OfPlayer)
					{
						Messages.Message("PW_FoundFossilOrEvoStone".Translate(user.LabelShortCap, thing.Label), thing, MessageTypeDefOf.PositiveEvent);
					}
				}
			}
		}
	}

	public static class FossilEvoStoneDropperUtility
    {
		
		public static ThingDef TryGetItem(float stoneDropRate, float fossilDropRate)
        {			
			if (Rand.Value <= stoneDropRate)
			{
				IEnumerable<ThingDef> evoStones = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.thingCategories != null && x.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("PW_EvolutionStone")));
				return evoStones.RandomElement();
			}
			if (Rand.Value <= fossilDropRate)
			{
				IEnumerable<ThingDef> fossils = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.thingCategories != null && x.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("PW_Fossils")));
				return fossils.RandomElement();				
			}
			return null;
		}
    }
}

