using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public static class PokemonUtility
    {
		public static bool FertileMateTarget(Pawn male, Pawn female)
		{
			CompPokemon compPokemonMale = male.TryGetComp<CompPokemon>();
			CompPokemon compPokemonFemale = female.TryGetComp<CompPokemon>();
			if (compPokemonMale == null || !compPokemonMale.friendshipTracker.CanMate() || compPokemonFemale == null || !compPokemonFemale.friendshipTracker.CanMate() || female.health.hediffSet.HasHediff(HediffDefOf.Sterilized))
            {
				return false;
			}
			EggGroupDef undiscovered = DefDatabase<EggGroupDef>.GetNamed("Undiscovered");
			EggGroupDef ditto = DefDatabase<EggGroupDef>.GetNamed("Ditto");
			if(compPokemonMale.eggGroups.Contains(undiscovered) || compPokemonFemale.eggGroups.Contains(undiscovered))
            {
				return false;
            }				
			if (compPokemonMale.eggGroups.Contains(ditto) && !compPokemonFemale.eggGroups.Contains(ditto) && female.gender != Gender.Male)
			{
				CompEggLayer compEggLayer = female.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					return !compEggLayer.FullyFertilized;
				}
			}
			else if(compPokemonFemale.eggGroups.Contains(ditto) && !compPokemonMale.eggGroups.Contains(ditto))
            {
				CompDittoEggLayer compDittoEggLayer = female.TryGetComp<CompDittoEggLayer>();
				if (compDittoEggLayer != null)
				{
					return !compDittoEggLayer.FullyFertilized;
				}
			}
			if (female.gender != Gender.Female)
			{
				return false;
			}
			foreach(EggGroupDef def in compPokemonMale.eggGroups)
            {
                if (compPokemonFemale.eggGroups.Contains(def))
                {
					CompEggLayer compEggLayer = female.TryGetComp<CompEggLayer>();
					if (compEggLayer != null)
					{
						return !compEggLayer.FullyFertilized;
					}
				}
            }      
			return false;
		}
	}
}
