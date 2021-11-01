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
    }
}
