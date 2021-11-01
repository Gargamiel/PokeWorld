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
	public class PawnKindColumnWorker_Info : PawnKindColumnWorker
	{
		public override void DoCell(Rect rect, PawnKindDef kindDef, PawnKindTable table)
		{
			if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(kindDef))
			{
				Widgets.InfoCardButton(rect.center.x - 12f, rect.center.y - 12f, kindDef.race);
			}
		}

		public override int GetMinWidth(PawnKindTable table)
		{
			return 24;
		}

		public override int GetMaxWidth(PawnKindTable table)
		{
			return 24;
		}
	}
}
