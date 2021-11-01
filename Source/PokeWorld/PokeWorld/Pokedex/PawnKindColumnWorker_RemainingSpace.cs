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
	public class PawnKindColumnWorker_RemainingSpace : PawnKindColumnWorker
	{
		public override void DoCell(Rect rect, PawnKindDef pawnKind, PawnKindTable table)
		{
		}

		public override int GetMinWidth(PawnKindTable table)
		{
			return 0;
		}

		public override int GetMaxWidth(PawnKindTable table)
		{
			return 1000000;
		}

		public override int GetOptimalWidth(PawnKindTable table)
		{
			return GetMaxWidth(table);
		}

		public override int GetMinCellHeight(PawnKindDef pawnKind)
		{
			return 0;
		}
	}
}
