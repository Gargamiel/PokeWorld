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
	public class PawnKindColumnWorker_PokedexIcon : PawnKindColumnWorker_Icon
	{
		protected override Texture2D GetIconFor(PawnKindDef pawnKind)
		{
            if (Find.World.GetComponent<PokedexManager>().IsPokemonSeen(pawnKind))
			{
				Texture2D Icon = ContentFinder<Texture2D>.Get(pawnKind.lifeStages[0].bodyGraphicData.texPath + "_east");
				return Icon;
			}
			return null;
		}
		protected override string GetIconTip(PawnKindDef pawnKind)
		{
			if (Find.World.GetComponent<PokedexManager>().IsPokemonSeen(pawnKind))
			{
				return pawnKind.label;
			}
			return null;
		}
	}
}
