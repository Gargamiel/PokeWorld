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
    class PawnColumnWorker_Caught : PawnColumnWorker_Icon
	{
		protected override Texture2D GetIconFor(Pawn pawn)
		{
			if (pawn.TryGetComp<CompPokemon>() != null && Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawn.kindDef))
			{
				return ContentFinder<Texture2D>.Get("Things/Item/Utility/Balls/PokeBall");
			}
			return null;
		}
		protected override string GetIconTip(Pawn pawn)
		{
			if (pawn.TryGetComp<CompPokemon>() != null && Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawn.kindDef))
			{
				return "PW_TipAlreadyCaught".Translate();
			}
			return null;
		}
		public override int Compare(Pawn a, Pawn b)
		{
			return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
		}

		private int GetValueToCompare(Pawn pawn)
		{
			CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			if (comp != null)
			{
				if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawn.kindDef))
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
			return 0;
		}
	}
}
