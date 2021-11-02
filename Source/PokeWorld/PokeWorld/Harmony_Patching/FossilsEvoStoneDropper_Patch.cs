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
					string defName = FossilEvoStoneDropperUtility.TryGetItemToSpawn(comp.stoneDropRate, comp.fossilDropRate, out string itemString);
					if (defName != null)
					{
						Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(defName));
						thing.stackCount = 1;
						List<Thing> list = __result.ToList();
						list.Add(thing);
						__result = list.AsEnumerable();
						if (__0 != null && __0.Faction == Faction.OfPlayer)
						{
							Messages.Message($"{__0.LabelShortCap} has found {itemString} !", thing, MessageTypeDefOf.PositiveEvent);
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
				string defName = FossilEvoStoneDropperUtility.TryGetItemToSpawn(comp.stoneDropRate, comp.fossilDropRate, out string itemString);
				if(defName != null)
                {
					Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(defName));
					thing.stackCount = 1;
					GenPlace.TryPlaceThing(thing, __instance.Position, __0, ThingPlaceMode.Near, ForbidIfNecessary);
					if (__3 != null && __3.Faction == Faction.OfPlayer)
                    {
						Messages.Message($"{__3.LabelShortCap} has found {itemString} !", thing, MessageTypeDefOf.PositiveEvent);
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
				string defName = FossilEvoStoneDropperUtility.TryGetItemToSpawn(comp.stoneDropRate, comp.fossilDropRate, out string itemString);
				if (defName != null)
				{
					Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(defName));
					thing.stackCount = 1;
					GenPlace.TryPlaceThing(thing, __instance.parent.Position, __instance.parent.Map, ThingPlaceMode.Near);
					Building building = __instance.parent as Building;
					Pawn user = building.InteractionCell.GetFirstPawn(building.Map);
					if (user != null && user.Faction == Faction.OfPlayer)
					{
						Messages.Message($"{user.LabelShortCap} has found {itemString} !", thing, MessageTypeDefOf.PositiveEvent);
					}
				}
			}
		}
	}

	public static class FossilEvoStoneDropperUtility
    {
		public static string TryGetItemToSpawn(float stoneDropRate, float fossilDropRate, out string itemString)
        {
			itemString = "";
			string defName = "";
			float num1 = Rand.Value;
			if (num1 <= stoneDropRate)
			{				
				itemString = "an Evolution Stone";
				float num2 = Rand.RangeInclusive(0, 7);
				switch (num2)
				{
					case 0:
						defName = "PW_FireStone";
						break;

					case 1:
						defName = "PW_IceStone";
						break;

					case 2:
						defName = "PW_LeafStone";
						break;

					case 3:
						defName = "PW_MoonStone";
						break;

					case 4:
						defName = "PW_ShinyStone";
						break;

					case 5:
						defName = "PW_ThunderStone";
						break;

					case 6:
						defName = "PW_WaterStone";
						break;

					case 7:
						defName = "PW_KingsRock";
						break;
				}
				return defName;
			}
			float num3 = Rand.Value;
			if (num3 <= fossilDropRate)
			{
				itemString = "a Fossil";
				float num4 = Rand.RangeInclusive(0, 6);
				switch (num4)
				{
					case 0:
						defName = "PW_HelixFossil";
						break;

					case 1:
						defName = "PW_DomeFossil";
						break;

					case 2:
						defName = "PW_OldAmber";
						break;

					case 3:
						defName = "PW_RootFossil";
						break;

					case 4:
						defName = "PW_ClawFossil";
						break;

					case 5:
						defName = "PW_SkullFossil";
						break;

					case 6:
						defName = "PW_ArmorFossil";
						break;
				}
				return defName;
			}
			return null;
		}
    }
}

