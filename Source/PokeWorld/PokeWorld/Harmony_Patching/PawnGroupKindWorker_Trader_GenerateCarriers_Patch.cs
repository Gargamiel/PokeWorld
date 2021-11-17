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
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader))]
    [HarmonyPatch("GenerateCarriers")]
    class PawnGroupKindWorker_Trader_GenerateCarriers_Patch
    {
        public static bool Prefix(PawnGroupMakerParms __0, PawnGroupMaker __1, Pawn __2, List<Thing> __3, List<Pawn> __4)
        {
            PawnKindDef kind = null;
            IEnumerable<PawnGenOption> kinds = null;
            if (!PokeWorldSettings.OkforPokemon() || PokeWorldSettings.allowNPCPokemonPack == false)
            {
                kind = __1.carriers.Where((PawnGenOption x) => __0.tile == -1 || (Find.WorldGrid[__0.tile].biome.IsPackAnimalAllowed(x.kind.race) && !x.kind.race.HasComp(typeof(CompPokemon)))).RandomElementByWeight((PawnGenOption x) => x.selectionWeight).kind;          
            }
            else
            {
                kinds = __1.carriers.Where((PawnGenOption x) => __0.tile == -1 || (Find.WorldGrid[__0.tile].biome.IsPackAnimalAllowed(x.kind.race) && x.kind.race.HasComp(typeof(CompPokemon)) && PokeWorldSettings.GenerationAllowed(x.kind.race.GetCompProperties<CompProperties_Pokemon>().generation)));
            }
            List<Thing> list = __3.Where((Thing x) => !(x is Pawn)).ToList();
            int i = 0;
            int num = Mathf.CeilToInt((float)list.Count / 8f);
            List<Pawn> list2 = new List<Pawn>();
            for (int j = 0; j < num; j++)
            {
                Pawn pawn;
                if (kind != null)
                {
                    pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, __0.faction, PawnGenerationContext.NonPlayer, __0.tile, forceGenerateNewPawn: false, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: true, allowAddictions: true, __0.inhabitants));
                }
                else
                {
                    pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kinds.RandomElementByWeight((PawnGenOption x) => x.selectionWeight).kind, __0.faction, PawnGenerationContext.NonPlayer, __0.tile, forceGenerateNewPawn: false, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: true, allowAddictions: true, __0.inhabitants));
                }
                if (i < list.Count)
                {
                    pawn.inventory.innerContainer.TryAdd(list[i]);
                    i++;
                }
                list2.Add(pawn);
                __4.Add(pawn);
            }
            for (; i < list.Count; i++)
            {
                list2.RandomElement().inventory.innerContainer.TryAdd(list[i]);
            }
            return false;
        }
    }
}