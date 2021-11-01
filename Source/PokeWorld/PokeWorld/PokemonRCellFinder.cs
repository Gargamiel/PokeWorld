using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PokeWorld
{
	public static class PokemonRCellFinder
	{
		public static IntVec3 BestOrderedGotoDestNear(IntVec3 root, Pawn searcher, Predicate<IntVec3> cellValidator = null)
		{
			Map map = searcher.Map;
			Predicate<IntVec3> predicate = delegate (IntVec3 c)
			{
				if (cellValidator != null && !cellValidator(c))
				{
					return false;
				}
				if (!map.pawnDestinationReservationManager.CanReserve(c, searcher, draftedOnly: false) || !c.Standable(map) || !searcher.CanReach(c, PathEndMode.OnCell, Danger.Deadly))
				{
					return false;
				}
				List<Thing> thingList = c.GetThingList(map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Pawn pawn = thingList[i] as Pawn;
					if (pawn != null && pawn != searcher && (pawn.RaceProps.Humanlike) && ((searcher.Faction.IsPlayer && pawn.Faction == searcher.Faction) || (!searcher.Faction.IsPlayer && !pawn.Faction.IsPlayer)))
					{
						return false;
					}
				}
				return true;
			};
			if (predicate(root))
			{
				return root;
			}
			int num = 1;
			IntVec3 result = default(IntVec3);
			float num2 = -1000f;
			bool flag = false;
			int num3 = GenRadial.NumCellsInRadius(30f);
			while (true)
			{
				IntVec3 intVec = root + GenRadial.RadialPattern[num];
				if (predicate(intVec))
				{
					float num4 = CoverUtility.TotalSurroundingCoverScore(intVec, map);
					if (num4 > num2)
					{
						num2 = num4;
						result = intVec;
						flag = true;
					}
				}
				if (num >= 8 && flag)
				{
					break;
				}
				num++;
				if (num >= num3)
				{
					return searcher.Position;
				}
			}
			return result;
		}
	}
}
