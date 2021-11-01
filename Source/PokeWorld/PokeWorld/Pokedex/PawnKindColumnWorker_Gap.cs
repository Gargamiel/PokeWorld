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
	public class PawnKindColumnWorker_Gap : PawnKindColumnWorker
	{
		protected virtual int Width => def.gap;

		public override void DoCell(Rect rect, PawnKindDef pawn, PawnKindTable table)
		{
		}

		public override int GetMinWidth(PawnKindTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), Width);
		}

		public override int GetMaxWidth(PawnKindTable table)
		{
			return Mathf.Min(base.GetMaxWidth(table), Width);
		}

		public override int GetMinCellHeight(PawnKindDef pawn)
		{
			return 0;
		}
	}
}
