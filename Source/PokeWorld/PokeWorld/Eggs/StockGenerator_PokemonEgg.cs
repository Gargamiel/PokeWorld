using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
	public class StockGenerator_PokemonEgg : StockGenerator
	{
		private ThingCategoryDef categoryDef;

		private IntRange thingDefCountRange = IntRange.one;

		private List<ThingDef> excludedThingDefs;

		private List<ThingCategoryDef> excludedCategories;

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			List<ThingDef> generatedDefs = new List<ThingDef>();
			int numThingDefsToUse = thingDefCountRange.RandomInRange;
			for (int i = 0; i < numThingDefsToUse; i++)
			{
				if (!categoryDef.DescendantThingDefs.Where((ThingDef t) => t.tradeability.TraderCanSell() && PokeWorldSettings.GenerationAllowed(t.GetCompProperties<CompProperties_PokemonEggHatcher>().hatcherPawn.race.GetCompProperties<CompProperties_Pokemon>().generation) && (int)t.techLevel <= (int)maxTechLevelGenerate && !generatedDefs.Contains(t) && (excludedThingDefs == null || !excludedThingDefs.Contains(t)) && (excludedCategories == null || !excludedCategories.Any((ThingCategoryDef c) => c.DescendantThingDefs.Contains(t)))).TryRandomElement(out var chosenThingDef))
				{
					break;
				}
				foreach (Thing item in StockGeneratorUtility.TryMakeForStock(chosenThingDef, RandomCountOf(chosenThingDef), faction))
				{
					yield return item;
				}
				generatedDefs.Add(chosenThingDef);
				chosenThingDef = null;
			}
		}

		public override bool HandlesThingDef(ThingDef t)
		{
			if (categoryDef.DescendantThingDefs.Contains(t) && t.HasComp(typeof(CompPokemonEggHatcher)) && PokeWorldSettings.GenerationAllowed(t.GetCompProperties<CompProperties_PokemonEggHatcher>().hatcherPawn.race.GetCompProperties<CompProperties_Pokemon>().generation) && t.tradeability != 0 && (int)t.techLevel <= (int)maxTechLevelBuy && (excludedThingDefs == null || !excludedThingDefs.Contains(t)))
			{
				if (excludedCategories != null)
				{
					return !excludedCategories.Any((ThingCategoryDef c) => c.DescendantThingDefs.Contains(t));
				}
				return true;
			}
			return false;
		}
	}
}
