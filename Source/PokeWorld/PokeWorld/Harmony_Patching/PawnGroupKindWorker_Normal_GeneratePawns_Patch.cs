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
		public static void Postfix(PawnGroupMakerParms __0, PawnGroupMaker __1, List<Pawn> __2, ref PawnGroupKindWorker_Normal __instance)
		{        
            Log.Error("PokeWorld_PawnGroupKindWorker_Normal_GeneratePawns_Patch");
			if (PokeWorldSettings.allowPokemonInRaid && PokeWorldSettings.OkforPokemon() && __2 != null && __2.Any() && (__0.raidStrategy == null || __0.raidStrategy != DefDatabase<RaidStrategyDef>.GetNamed("Siege")))
            {
                Log.Error("1");
                int maxPokes = 1 + (int)Math.Round(Rand.Range(__2.Count() / 4f, __2.Count() / 1.5f));
                float pointsLeft = __0.points;
                Log.Error("pointsLeft:" + pointsLeft.ToString());
                List<Pawn> list = new List<Pawn>();
                for (int i = 0; i < maxPokes; i++)
                {
                    if(DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.race.HasComp(typeof(CompPokemon)) && PokeWorldSettings.GenerationAllowed(x.race.GetCompProperties<CompProperties_Pokemon>().generation) && x.combatPower <= pointsLeft && (x.race.GetCompProperties<CompProperties_Pokemon>().attributes == null || !x.race.GetCompProperties<CompProperties_Pokemon>().attributes.Any())).TryRandomElementByWeight((PawnKindDef x) => 1 + (1 / x.race.GetCompProperties<CompProperties_Pokemon>().rarity), out PawnKindDef kind))
                    {
                        Log.Error(kind.ToString());
                        Pawn pawn = PawnGenerator.GeneratePawn(kind, __0.faction);
                        list.Add(pawn);
                        pointsLeft -= kind.combatPower;
                    }
                    if(pointsLeft < 0)
                    {
                        break;
                    }
                }
                if (list.Any())
                {
                    foreach(Pawn pawn in list)
                    {
                        __2.Add(pawn);
                    }
                    PawnGroupKindWorker.pawnsBeingGeneratedNow.Add(list);                    
                }               
            }
		}
    }
    */
}
