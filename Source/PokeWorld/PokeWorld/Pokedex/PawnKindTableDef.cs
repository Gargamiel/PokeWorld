using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;


namespace PokeWorld
{	
	public class PawnKindTableDef : Def
	{
		public List<PawnKindColumnDef> columns;

		public Type workerClass = typeof(PawnKindTable);

		public int minWidth = 500;
	}
}
