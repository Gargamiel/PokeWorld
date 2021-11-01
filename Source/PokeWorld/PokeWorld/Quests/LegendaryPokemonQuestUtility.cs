using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
	public static class LegendaryPokemonQuestUtility
	{
		public static Pawn GenerateLegendaryPokemon(int tile, PawnKindDef pawnKind = null)
		{
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, tile: tile));
			pawn.health.Reset();
			return pawn;
		}
	}
}
