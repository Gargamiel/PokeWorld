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
    [HarmonyPatch(typeof(ResearchManager))]
    [HarmonyPatch("FinishProject")]
    class ResearchManager_FinishProject_Patch
    {
        static void Postfix(ResearchProjectDef __0, Pawn __2)
        {          
            if(__0.defName == "PW_CyberspaceProgramming" || __0.defName == "PW_PlanetaryDevelopment" || __0.defName == "PW_ExtraDimensionalActivity")
            {
                Pawn pawn = __2;
                if (pawn != null)
                {
                    IntVec3 pos = pawn.Position;
                    Map map = pawn.Map;
                    switch (__0.defName)
                    {
                        case "PW_CyberspaceProgramming":
                            PokemonGeneratorUtility.GenerateAndSpawnNewPokemon(DefDatabase<PawnKindDef>.GetNamed("PW_Porygon"), Faction.OfPlayer, pos, map, pawn, true);
                            Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(DefDatabase<PawnKindDef>.GetNamed("PW_Porygon"));
                            break;

                        case "PW_PlanetaryDevelopment":
                            Thing item1 = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("PW_Upgrade"));
                            GenSpawn.Spawn(item1, pos, map);
                            break;

                        case "PW_ExtraDimensionalActivity":
                            Thing item2 = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("PW_DubiousDisc"));
                            GenSpawn.Spawn(item2, pos, map);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Log.Error("Researched without having an active pawn researcher.");
                    return;
                }
            }                                   
        }
    }
}
