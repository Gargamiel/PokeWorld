using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
	public class Recipe_RareCandy : Recipe_Surgery
	{
		//Surgery recipe, not crafting recipe
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				CompPokemon comp = pawn.TryGetComp<CompPokemon>();
				if (comp != null && comp.levelTracker != null)
				{
					for (int i = 0; i < bill.recipe.ingredients[0].GetBaseCount(); i++)
					{
						comp.levelTracker.IncreaseExperience(comp.levelTracker.totalExpForNextLevel-comp.levelTracker.experience);
					}
				}				
			}
		}
	}
}
