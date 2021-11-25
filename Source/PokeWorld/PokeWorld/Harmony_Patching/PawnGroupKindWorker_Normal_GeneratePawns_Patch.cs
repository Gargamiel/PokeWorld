using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace PokeWorld
{
    /*
    [HarmonyPatch(typeof(PawnGroupKindWorker_Normal))]
    [HarmonyPatch("GeneratePawns")]
    class PawnGroupKindWorker_Normal_GeneratePawns_Patch
    {
		public static void postfix(PawnGroupMakerParms __0, PawnGroupMaker __1, List<Pawn> __2, bool __3)
		{
            Log.Error("1");
			if (PokeWorldSettings.allowPokemonInRaid && __2.Any() && (__0.raidStrategy == null || __0.raidStrategy != DefDatabase<RaidStrategyDef>.GetNamed("Siege")))
            {
                int maxPokes = (int)Rand.Range(__2.Count / 4f, __2.Count / 2f);
                for (int i = 0; i < maxPokes; i++)
                {
                    PawnKindDef kind = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.race.HasComp(typeof(CompPokemon)) && PokeWorldSettings.GenerationAllowed(x.race.GetCompProperties<CompProperties_Pokemon>().generation) && !x.race.GetCompProperties<CompProperties_Pokemon>().attributes.Contains(PokemonAttribute.Baby) && !x.race.GetCompProperties<CompProperties_Pokemon>().attributes.Contains(PokemonAttribute.Fossil) && !x.race.GetCompProperties<CompProperties_Pokemon>().attributes.Contains(PokemonAttribute.Legendary) && !x.race.GetCompProperties<CompProperties_Pokemon>().attributes.Contains(PokemonAttribute.Particular)).RandomElementByWeight((PawnKindDef x) => 1 / x.race.GetCompProperties<CompProperties_Pokemon>().rarity);
                    Pawn pawn = PawnGenerator.GeneratePawn(kind, __0.faction);
                    __2.Add(pawn);
                }
            }			
		}
	}
    */
}
