using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace PokeWorld
{
	[StaticConstructorOnStartup]
	public class PawnKindColumnWorker_PokedexStatus : PawnKindColumnWorker_Icon
	{
		private static readonly Texture2D caughtIcon = ContentFinder<Texture2D>.Get("UI/Icon/PokeBall");
		private static readonly Texture2D seenIcon = ContentFinder<Texture2D>.Get("UI/Icon/Seen");
		protected override Texture2D GetIconFor(PawnKindDef pawnKind)
		{
			if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawnKind))
			{
				return caughtIcon;
			}
			else if (Find.World.GetComponent<PokedexManager>().IsPokemonSeen(pawnKind))
            {
				return seenIcon;
			}
			return null;
		}
	}
}
