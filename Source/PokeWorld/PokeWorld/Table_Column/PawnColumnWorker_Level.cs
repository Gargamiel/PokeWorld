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
	public class PawnColumnWorker_Level : PawnColumnWorker_Text
	{
        protected override string GetTextFor(Pawn pawn)
        {
			CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			if (comp != null)
			{
				return "PW_LevelShort".Translate(comp.levelTracker.level);
			}
			return "";
		}

        protected override string GetTip(Pawn pawn)
        {
			CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			if (comp != null)
			{
				return "PW_LevelLong".Translate(comp.levelTracker.level);
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
				return comp.levelTracker.level;
			}
			return 0;
		}
	}
}
