using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace PokeWorld
{
    public class QuestConditionDef : Def
    {
        public QuestScriptDef questScriptDef;
        public List<GenerationRequirement> generationsRequirements;
        public List<PawnKindDef> requiredKindSeen;
        public int requiredSeenMinCount = -1;
        public List<PawnKindDef> requiredKindCaught;
        public int requiredCaughtMinCount = -1;

        public bool CheckCompletion()
        {
            PokedexManager pokedex = Find.World.GetComponent<PokedexManager>();
            if(requiredKindSeen != null)
            {
                if(requiredSeenMinCount == -1)
                {
                    requiredSeenMinCount = requiredKindSeen.Count();
                }
                int count = 0;
                foreach (PawnKindDef kindDef in requiredKindSeen)
                {
                    if (pokedex.IsPokemonSeen(kindDef))
                    {
                        count += 1;
                    }
                }
                if(count < requiredSeenMinCount)
                {
                    return false;
                }
            }
            if(requiredKindCaught != null)
            {
                if(requiredCaughtMinCount == -1)
                {
                    requiredCaughtMinCount = requiredKindCaught.Count();
                }
                int count = 0;
                foreach (PawnKindDef kindDef in requiredKindCaught)
                {
                    if (pokedex.IsPokemonCaught(kindDef))
                    {
                        count += 1;
                    }
                }
                if (count < requiredCaughtMinCount)
                {
                    return false;
                }
            }
            if(generationsRequirements != null)
            {
                foreach(GenerationRequirement genReq in generationsRequirements)
                {
                    if(pokedex.TotalSeen(genReq.generation, false) < genReq.minNoLegSeen || pokedex.TotalSeen(genReq.generation) < genReq.minSeen || pokedex.TotalCaught(genReq.generation, false) < genReq.minNoLegCaught || pokedex.TotalCaught(genReq.generation) < genReq.minCaught)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
