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
    [HarmonyPatch(typeof(IncidentWorker_FarmAnimalsWanderIn))]
    [HarmonyPatch("TryFindRandomPawnKind")]
    class IncidentWorker_FarmAnimalsWanderIn_TryFindRandomPawnKind_Patch
    {
        public static void Postfix(Map __0, out PawnKindDef __1, ref bool __result)
        {
            if (PokeWorldSettings.OkforPokemon())
            {
                __1 = null;
                __result = false;
            }
            else
            {
                __result = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.RaceProps.Animal && !x.race.HasComp(typeof(CompPokemon)) && x.RaceProps.wildness < 0.35f && __0.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race)).TryRandomElementByWeight((PawnKindDef k) => 0.420000017f - k.RaceProps.wildness, out __1);
            }
        }
    }

    [HarmonyPatch(typeof(ManhunterPackIncidentUtility))]
    [HarmonyPatch("TryFindManhunterAnimalKind")]
    class ManhunterPackIncidentUtility_TryFindManhunterAnimalKind_Patch
    {
        public static void Postfix(float __0, int __1, out PawnKindDef __2, ref bool __result)
        {
            if (PokeWorldSettings.OkforPokemon())
            {
                __2 = null;
                __result = false;
            }
            else
            {
                IEnumerable<PawnKindDef> source = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef k) => k.RaceProps.Animal && !k.race.HasComp(typeof(CompPokemon)) && k.canArriveManhunter && (__1 == -1 || Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(__1, k.race)));
                if (source.Any())
                {
                    if (source.TryRandomElementByWeight((PawnKindDef a) => ManhunterPackIncidentUtility.ManhunterAnimalWeight(a, __0), out __2))
                    {
                        __result = true;
                    }
                    else if (__0 > source.Min((PawnKindDef a) => a.combatPower) * 2f)
                    {
                        __2 = source.MaxBy((PawnKindDef a) => a.combatPower);
                        __result = true;
                    }
                    else
                    {
                        __2 = null;
                        __result = false;
                    }
                }
                else
                {
                    __2 = null;
                    __result = false;
                }                
            }            
        }
    }
    [HarmonyPatch(typeof(IncidentWorker_Infestation))]
    [HarmonyPatch("TryExecuteWorker")]
    class IncidentWorker_Infestation_TryExecuteWorker_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (PokeWorldSettings.OkforPokemon())
            {
                __result = false;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(IncidentWorker_DeepDrillInfestation))]
    [HarmonyPatch("TryExecuteWorker")]
    class IncidentWorker_DeepDrillInfestation_TryExecuteWorker_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (PokeWorldSettings.OkforPokemon())
            {
                __result = false;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(IncidentWorker_ThrumboPasses))]
    [HarmonyPatch("TryExecuteWorker")]
    class IncidentWorker_ThrumboPasses_TryExecuteWorker_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            if (PokeWorldSettings.OkforPokemon())
            {
                __result = false;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    //For alphabeaver event (work with debug command)
    [HarmonyPatch(typeof(Storyteller))]
    [HarmonyPatch("TryFire")]
    class IncidentWorker_HerdMigration_TryFindAnimalKind_Patch
    {
        public static bool Prefix(FiringIncident __0, ref bool __result)
        {
            if(__0.def.Worker.def == DefDatabase<IncidentDef>.GetNamed("Alphabeavers"))
            {
                if (PokeWorldSettings.OkforPokemon())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(IncidentWorker_SelfTame))]
    [HarmonyPatch("Candidates")]
    class IncidentWorker_SelfTame_Candidates_Patch
    {
        public static void Postfix(Map __0, ref IEnumerable<Pawn> __result)
        {
            __result = __0.mapPawns.AllPawnsSpawned.Where((Pawn x) => x.RaceProps.Animal && x.Faction == null && x.TryGetComp<CompPokemon>() == null && !x.Position.Fogged(x.Map) && !x.InMentalState && !x.Downed && x.RaceProps.wildness > 0f);
        }
    }
}
