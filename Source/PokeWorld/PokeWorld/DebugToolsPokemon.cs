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
		[DebugAction("Pokeworld", "+1 level", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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

		[DebugAction("Pokeworld", "Max level", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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

		[DebugAction("Pokeworld", "+ 20.000 Xp", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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

		[DebugAction("Pokeworld", "Make Shiny", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
		[DebugAction("Pokeworld", "+ 20 friendship", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
		[DebugAction("Pokeworld", "- 20 friendship", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
		[DebugAction("Pokeworld", "Hatch Pokémon Egg", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
		[DebugAction("Pokeworld", "Fill Pokédex", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
		private static void FillPokedex()
		{
			Find.World.GetComponent<PokedexManager>().DebugFillPokedex();
		}
		[DebugAction("Pokeworld", "Show hidden stats", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
						text += $"{pawn.ThingID} stats:\nIV: {hpIV} {atIV} {defIV} {spatIV} {spdefIV} {spdIV}, EV: {hpEV} {atEV} {defEV} {spatEV} {spdefEV} {spdEV}";
						Log.Message(text);
					}
				}
			}
		}
	}
}
