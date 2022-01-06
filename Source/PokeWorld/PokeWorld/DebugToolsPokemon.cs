using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace PokeWorld
{
    public static class DebugToolsPokemon
    {
		[DebugAction("PokéWorld", "+1 level", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void Add1Level()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn)
				{
					CompPokemon comp = ((Pawn)item).TryGetComp<CompPokemon>();
					if (comp != null)
                    {
						comp.levelTracker.IncreaseExperience(comp.levelTracker.totalExpForNextLevel - comp.levelTracker.experience);
                    }
				}
			}
		}

		[DebugAction("PokéWorld", "Max level", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeMaxLevel()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn)
				{
					CompPokemon comp = ((Pawn)item).TryGetComp<CompPokemon>();
					if (comp != null)
					{
						comp.levelTracker.IncreaseExperience(2000000);
					}
				}
			}
		}

		[DebugAction("PokéWorld", "+ 20.000 Xp", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void Give20000Xp()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn)
				{
					CompPokemon comp = ((Pawn)item).TryGetComp<CompPokemon>();
					if (comp != null)
					{
						comp.levelTracker.IncreaseExperience(20000);
					}
				}
			}
		}

		[DebugAction("PokéWorld", "Make Shiny", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeShiny()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn pawn)
				{
					CompPokemon comp = pawn.TryGetComp<CompPokemon>();
					if (comp != null && comp.shinyTracker != null)
					{
						comp.shinyTracker.MakeShiny();
						pawn.Drawer.renderer.graphics.ResolveAllGraphics();
					}
				}
			}
		}
		[DebugAction("PokéWorld", "+ 20 friendship", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void Give20Friendship()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn pawn)
				{
					CompPokemon comp = pawn.TryGetComp<CompPokemon>();
					if (comp != null && comp.friendshipTracker != null)
					{
						comp.friendshipTracker.ChangeFriendship(20);
					}
				}
			}
		}
		[DebugAction("PokéWorld", "- 20 friendship", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void Remove20Friendship()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn pawn)
				{
					CompPokemon comp = pawn.TryGetComp<CompPokemon>();
					if (comp != null && comp.friendshipTracker != null)
					{
						comp.friendshipTracker.ChangeFriendship(-20);
					}
				}
			}
		}
		[DebugAction("PokéWorld", "Hatch Pokémon Egg", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void HatchEgg()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is ThingWithComps thingWithComps && thingWithComps.TryGetComp<CompPokemonEggHatcher>() != null)
                {
					CompPokemonEggHatcher comp = thingWithComps.TryGetComp<CompPokemonEggHatcher>();
					comp.HatchPokemon();
				}
			}
		}
		[DebugAction("PokéWorld", "Fill Pokédex", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
		private static void FillPokedex()
		{
			Find.World.GetComponent<PokedexManager>().DebugFillPokedex();
		}
		[DebugAction("PokéWorld", "Fill Pokédex no legendary", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
		private static void FillPokedexNoLegendary()
		{
			Find.World.GetComponent<PokedexManager>().DebugFillPokedexNoLegendary();
		}
		[DebugAction("PokéWorld", "Show hidden stats", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void ShowHiddenStat()
		{
			foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
			{
				if (item is Pawn pawn)
				{
					CompPokemon comp = pawn.TryGetComp<CompPokemon>();
					if (comp != null && comp.statTracker != null)
					{
						string text = "";
						StatDef hpStatDef = DefDatabase<StatDef>.GetNamed("PW_HP");
						StatDef attackStatDef = DefDatabase<StatDef>.GetNamed("PW_Attack");
						StatDef defenseStatDef = DefDatabase<StatDef>.GetNamed("PW_Defense");
						StatDef spAttackStatDef = DefDatabase<StatDef>.GetNamed("PW_SpecialAttack");
						StatDef spDefenseStatDef = DefDatabase<StatDef>.GetNamed("PW_SpecialDefense");
						StatDef speedStatDef = DefDatabase<StatDef>.GetNamed("PW_Speed");
						int hpIV = comp.statTracker.GetIV(hpStatDef);
						int atIV = comp.statTracker.GetIV(attackStatDef);
						int defIV = comp.statTracker.GetIV(defenseStatDef);
						int spatIV = comp.statTracker.GetIV(spAttackStatDef);
						int spdefIV = comp.statTracker.GetIV(spDefenseStatDef);
						int spdIV = comp.statTracker.GetIV(speedStatDef);
						int hpEV = comp.statTracker.GetEV(hpStatDef);
						int atEV = comp.statTracker.GetEV(attackStatDef);
						int defEV = comp.statTracker.GetEV(defenseStatDef);
						int spatEV = comp.statTracker.GetEV(spAttackStatDef);
						int spdefEV = comp.statTracker.GetEV(spDefenseStatDef);
						int spdEV = comp.statTracker.GetEV(speedStatDef);
						text += $"{pawn.ThingID} stats:\nIV: hp:{hpIV} att:{atIV} def:{defIV} spAt:{spatIV} spDef:{spdefIV} spd:{spdIV}\nEV: hp:{hpEV} att:{atEV} def:{defEV} spAt:{spatEV} spDef:{spdefEV} spd:{spdEV}";
						Log.Message(text);
					}
				}
			}
		}
		[DebugAction("PokéWorld", "Make colony (Gen 1)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen1()
		{
			Autotests_PokemonColonyMaker.MakeColony(1);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 2)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen2()
		{
			Autotests_PokemonColonyMaker.MakeColony(2);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 3)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen3()
		{
			Autotests_PokemonColonyMaker.MakeColony(3);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 4)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen4()
		{
			Autotests_PokemonColonyMaker.MakeColony(4);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 5)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen5()
		{
			Autotests_PokemonColonyMaker.MakeColony(5);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 6)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen6()
		{
			Autotests_PokemonColonyMaker.MakeColony(6);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 7)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen7()
		{
			Autotests_PokemonColonyMaker.MakeColony(7);
		}
		[DebugAction("PokéWorld", "Make colony (Gen 8)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyGen8()
		{
			Autotests_PokemonColonyMaker.MakeColony(8);
		}
		[DebugAction("PokéWorld", "Make colony (All Pokémon)", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void MakeColonyAllPokemon()
		{
			Autotests_PokemonColonyMaker.MakeColonyAll();
		}
	}
	public static class Autotests_PokemonColonyMaker
    {
		private static CellRect overRect;

		private static BoolGrid usedCells;

		private const int OverRectSize = 100;

		private static Map Map => Find.CurrentMap;
		public static void MakeColony(int generation)
		{
			bool godMode = DebugSettings.godMode;
			DebugSettings.godMode = true;
			Thing.allowDestroyNonDestroyable = true;
			if (usedCells == null)
			{
				usedCells = new BoolGrid(Map);
			}
			else
			{
				usedCells.ClearAndResizeTo(Map);
			}
			overRect = new CellRect(Map.Center.x - 50, Map.Center.z - 50, 100, 100);
			DeleteAllSpawnedPawns();
			GenDebug.ClearArea(overRect, Find.CurrentMap);
			
			foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef k) => k.RaceProps.Animal && k.race.HasComp(typeof(CompPokemon)) && k.race.GetCompProperties<CompProperties_Pokemon>().generation == generation))
			{
				if (!TryGetFreeRect(6, 3, out var result))
				{
					return;
				}
				result = result.ContractedBy(1);
				foreach (IntVec3 item2 in result)
				{
					Map.terrainGrid.SetTerrain(item2, TerrainDefOf.Concrete);
				}
				GenSpawn.Spawn(PawnGenerator.GeneratePawn(item), result.Cells.ElementAt(0), Map);
				IntVec3 intVec = result.Cells.ElementAt(1);
				HealthUtility.DamageUntilDead((Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(item), intVec, Map));
				CompRottable compRottable = ((Corpse)intVec.GetThingList(Find.CurrentMap).First((Thing t) => t is Corpse)).TryGetComp<CompRottable>();
				if (compRottable != null)
				{
					compRottable.RotProgress += 1200000f;
				}
				if (item.RaceProps.leatherDef != null)
				{
					GenSpawn.Spawn(item.RaceProps.leatherDef, result.Cells.ElementAt(2), Map);
				}
				if (item.RaceProps.meatDef != null)
				{
					GenSpawn.Spawn(item.RaceProps.meatDef, result.Cells.ElementAt(3), Map);
				}
			}
			if (!TryGetFreeRect(33, 33, out var result2))
			{
				Log.Error("Could not get wallable rect");
			}
			result2 = result2.ContractedBy(1);
			ClearAllHomeArea();
			FillWithHomeArea(overRect);
			DebugSettings.godMode = godMode;
			Thing.allowDestroyNonDestroyable = false;
		}
		public static void MakeColonyAll()
		{
			bool godMode = DebugSettings.godMode;
			DebugSettings.godMode = true;
			Thing.allowDestroyNonDestroyable = true;
			if (usedCells == null)
			{
				usedCells = new BoolGrid(Map);
			}
			else
			{
				usedCells.ClearAndResizeTo(Map);
			}
			overRect = new CellRect(Map.Center.x - 50, Map.Center.z - 50, 100, 100);
			DeleteAllSpawnedPawns();
			GenDebug.ClearArea(overRect, Find.CurrentMap);

			foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef k) => k.RaceProps.Animal && k.race.HasComp(typeof(CompPokemon))))
			{
				if (!TryGetFreeRect(3, 3, out var result))
				{
					return;
				}
				result = result.ContractedBy(1);
				foreach (IntVec3 item2 in result)
				{
					Map.terrainGrid.SetTerrain(item2, TerrainDefOf.Concrete);
				}
				Pawn pawn = PawnGenerator.GeneratePawn(item);
				GenSpawn.Spawn(pawn, result.Cells.ElementAt(0), Map);
				pawn.rotationTracker.FaceCell(result.BottomLeft);
			}
			if (!TryGetFreeRect(33, 33, out var result2))
			{
				Log.Error("Could not get wallable rect");
			}
			result2 = result2.ContractedBy(1);
			ClearAllHomeArea();
			FillWithHomeArea(overRect);
			DebugSettings.godMode = godMode;
			Thing.allowDestroyNonDestroyable = false;
		}
		private static bool TryGetFreeRect(int width, int height, out CellRect result)
		{
			for (int i = overRect.minZ; i <= overRect.maxZ - height; i++)
			{
				for (int j = overRect.minX; j <= overRect.maxX - width; j++)
				{
					CellRect cellRect = new CellRect(j, i, width, height);
					bool flag = true;
					for (int k = cellRect.minZ; k <= cellRect.maxZ; k++)
					{
						for (int l = cellRect.minX; l <= cellRect.maxX; l++)
						{
							if (usedCells[l, k])
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
					result = cellRect;
					for (int m = cellRect.minZ; m <= cellRect.maxZ; m++)
					{
						for (int n = cellRect.minX; n <= cellRect.maxX; n++)
						{
							IntVec3 c = new IntVec3(n, 0, m);
							usedCells.Set(c, value: true);
							if (c.GetTerrain(Find.CurrentMap).passability == Traversability.Impassable)
							{
								Map.terrainGrid.SetTerrain(c, TerrainDefOf.Concrete);
							}
						}
					}
					return true;
				}
			}
			result = new CellRect(0, 0, width, height);
			return false;
		}
		private static void DeleteAllSpawnedPawns()
		{
			foreach (Pawn item in Map.mapPawns.AllPawnsSpawned.ToList())
			{
				item.Destroy();
				item.relations.ClearAllRelations();
			}
			Find.GameEnder.gameEnding = false;
		}
		private static void ClearAllHomeArea()
		{
			foreach (IntVec3 allCell in Map.AllCells)
			{
				Map.areaManager.Home[allCell] = false;
			}
		}

		private static void FillWithHomeArea(CellRect r)
		{
			new Designator_AreaHomeExpand().DesignateMultiCell(r.Cells);
		}
	}
}
