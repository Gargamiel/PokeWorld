using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using UnityEngine;

namespace PokeWorld
{
	
	public class GenStep_LegendaryPokemon : GenStep_Scatterer
	{
		public override int SeedPart => 860042045;

		protected override bool CanScatterAt(IntVec3 c, Map map)
		{
			if (!base.CanScatterAt(c, map))
			{
				return false;
			}
			if (!c.Standable(map))
			{
				return false;
			}
			if (c.Roofed(map))
			{
				return false;
			}
			if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors)))
			{
				return false;
			}
			return true;
		}

		protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
		{
			Pawn pawn = (Pawn)parms.sitePart.things.Take(parms.sitePart.things[0]);		
			GenSpawn.Spawn(pawn, loc, map);
			pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
			MapGenerator.rootsToUnfog.Add(loc);
			MapGenerator.SetVar("RectOfInterest", CellRect.CenteredOn(loc, 1, 1));
		}
	}
	
}
