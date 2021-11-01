using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public static class PokemonGeneratorUtility
    {
        public static Pawn GenerateAndSpawnNewPokemon(PawnKindDef kind, Faction faction, IntVec3 position, Map map, Pawn trainer = null, bool newBorn = false, bool fossil = false)
        {
            Pawn Pokemon = PawnGenerator.GeneratePawn(kind, faction);            
            if (newBorn)
            {
                Pokemon.ageTracker.BirthAbsTicks = GenTicks.TicksAbs;
                Pokemon.ageTracker.AgeBiologicalTicks = 0;
                if (fossil)
                {
                    Pokemon.ageTracker.AgeChronologicalTicks = (long)Rand.Range(237600000000000, 400000000000000) ;
                }
                else
                {
                    Pokemon.ageTracker.AgeChronologicalTicks = 0;
                }
                Pokemon.health.Reset();
            }
            GenSpawn.Spawn(Pokemon, position, map);
            if(trainer != null)
            {
                Pokemon.training.Train(DefDatabase<TrainableDef>.GetNamed("Obedience"), trainer, true);
                Pokemon.training.SetWantedRecursive(DefDatabase<TrainableDef>.GetNamed("Obedience"), true);
            }     
            return Pokemon;
        }
    }
}
