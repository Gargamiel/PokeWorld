using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
	public class PawnKindTable_Pokedex : PawnKindTable
	{
		protected override IEnumerable<PawnKindDef> LabelSortFunction(IEnumerable<PawnKindDef> input)
		{
			return from p in input
				   orderby p.race.GetCompProperties<CompProperties_Pokemon>().pokedexNumber
				   select p;
		}

		protected override IEnumerable<PawnKindDef> PrimarySortFunction(IEnumerable<PawnKindDef> input)
		{
			return from p in input
				   orderby p.race.GetCompProperties<CompProperties_Pokemon>().pokedexNumber
				   select p;
		}

		public PawnKindTable_Pokedex(PawnKindTableDef def, Func<IEnumerable<PawnKindDef>> pawnKindsGetter, int uiWidth, int uiHeight)
			: base(def, pawnKindsGetter, uiWidth, uiHeight)
		{
		}
	}
}
