using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace PokeWorld
{
    //Surgery recipe, not crafting recipe
    public class Recipe_EvolutionItem : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                pawn.GetComp<CompPokemon>().levelTracker.TryEvolveWithItem(ingredients[0]);
            }
        }
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if(thing is Pawn pawn)
            {
                CompPokemon comp = pawn.TryGetComp<CompPokemon>();
                if (comp != null && comp.evolutions != null) {
                    foreach(Evolution evo in comp.evolutions)
                    {
                        if (PokeWorldSettings.GenerationAllowed(evo.evolution.race.GetCompProperties<CompProperties_Pokemon>().generation))
                        {
                            return true;
                        }
                    }                    
                }
            }
            return false;
        }
    }
}
