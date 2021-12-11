using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
	public class Recipe_HealPowder : Recipe_Surgery
	{
		//Surgery recipe, not crafting recipe
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				HealthUtility.FixWorstHealthCondition(pawn);
				CompPokemon comp = pawn.TryGetComp<CompPokemon>();
                if (comp != null && pawn.Faction.IsPlayer)
                {
					comp.friendshipTracker.ChangeFriendship(-6);
                }
			}
		}
	}
}
