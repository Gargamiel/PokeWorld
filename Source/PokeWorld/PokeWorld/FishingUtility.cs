using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace PokeWorld
{
    public static class FishingUtility
    {
        public static int ComputeFishingTicks(Pawn pawn, Thing fishingRod)
        {
            int ticks = (int)(1200f * (1f / pawn.GetStatValue(StatDef.Named("PW_FishingSpeed"))) * (1f / fishingRod.GetStatValue(StatDef.Named("PW_FishingSpeedMultipler"))));
            return ticks;
        }
        public static float ComputeLineBreakChance(Pawn pawn, Thing fishingRod)
        {
            float pawnBreakProb = pawn.GetStatValue(StatDef.Named("PW_FishingLineBreak"));
            float rodStrenght = fishingRod.GetStatValue(StatDef.Named("PW_FishingLineStrenghtMultiplier"));
            if (rodStrenght >= 1){
                return pawnBreakProb * (1 / rodStrenght);
            }
            return 1 - ((1 - pawnBreakProb) * (rodStrenght));
        }
        public static bool TryFish(Pawn pawn, Thing fishingRod, IntVec3 cell)
        {
            if (Rand.Chance(ComputeLineBreakChance(pawn, fishingRod))){
                MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "Line broke");
                return false;
            }
            PawnKindDef kind = DefDatabase<FishingRateDef>.GetRandom().getRandomFish(pawn.Map.Biome, cell.GetTerrain(pawn.Map), fishingRod.def);
            Pawn pokemon = PokemonGeneratorUtility.GenerateAndSpawnNewPokemon(kind, null, cell, pawn.Map);
            pokemon.caller.DoCall();
            return true;
        }

        public static bool IsFishingTerrain(TerrainDef terrain)
        {
            return new[] {TerrainDefOf.WaterShallow,
                      TerrainDefOf.WaterDeep,
                      TerrainDefOf.WaterMovingShallow,
                      TerrainDefOf.WaterMovingChestDeep,
                      TerrainDefOf.WaterOceanShallow,
                      TerrainDefOf.WaterOceanDeep,
                      TerrainDef.Named("Marsh")
                    }.Contains(terrain);
        }
    }
}
