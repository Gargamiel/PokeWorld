using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace PokeWorld
{
	[HarmonyPatch(typeof(Pawn))]
	[HarmonyPatch("GetExtraFloatMenuOptionsFor")]
	class Pawn_GetExtraFloatMenuOptionsFor_Patch
	{
		public static void Postfix(Pawn __instance, IntVec3 __0, ref IEnumerable<FloatMenuOption> __result)
		{
            if (__instance.Drafted)
            {
				return;
            }
			Thing fishingRod = null;
			foreach (Thing thing in __instance.EquippedWornOrInventoryThings)
            {
				if(thing.TryGetComp<CompFishingRod>() != null)
                {
					fishingRod = thing;
					break;
                }
            }
			/*if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {

            }*/

			if (fishingRod == null)
            {
				return;
            }
			TerrainDef targetTerrain = __0.GetTerrain(__instance.Map);
			if (!FishingUtility.IsFishingTerrain(targetTerrain))
            {
				return;
            }      
			if (!ReachabilityUtility.CanReach(__instance, __0, PathEndMode.Touch, Danger.Unspecified))
            {
				__result = __result.AddItem(new FloatMenuOption("Cannot reach fishing spot", null));
				return;
            }
            else
            {
				Action action = getFishingAction(__instance, __0, fishingRod);
				__result = __result.AddItem(new FloatMenuOption($"Fish here ({targetTerrain.label})", action));
			}
			
		}

		private static Action getFishingAction(Pawn pawn, IntVec3 targetTerrain, Thing fishingRod)
        {
			Action action = delegate
			{
				Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_Fish"), targetTerrain);
				job.targetB = fishingRod;
				pawn.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
			};
			return action;
        }
	}
}