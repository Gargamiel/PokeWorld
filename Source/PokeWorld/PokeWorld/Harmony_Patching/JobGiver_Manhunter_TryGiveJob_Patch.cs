using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using System.Reflection;

namespace PokeWorld
{
	[HarmonyPatch(typeof(JobGiver_Manhunter))]
	[HarmonyPatch("TryGiveJob")]
	class JobGiver_Manhunter_TryGiveJob_Patch
	{
		public static readonly IntRange ExpiryInterval_ShooterSucceeded = new IntRange(450, 550);
		private static readonly IntRange ExpiryInterval_Melee = new IntRange(360, 480);
		private static float targetKeepRadius = 65f;
		private static bool Prefix(Pawn __0, ref Job __result)
		{
			CompPokemon comp = __0.TryGetComp<CompPokemon>();
			if(comp == null)
            {
				return true;
            }
			UpdateEnemyTarget(__0);
			Thing enemyTarget = __0.mindState.enemyTarget;
			if (enemyTarget == null)
			{
				__result = null;
				return false;
			}
			Pawn pawn2 = enemyTarget as Pawn;
			if (pawn2 != null && pawn2.IsInvisible())
			{
				__result = null;
				return false;
			}
			Verb verb = __0.TryGetAttackVerb(enemyTarget);
			if (verb == null)
			{
				__result = null;
				return false;
			}
			if (verb.tool != null)
			{
				__result = MeleeAttackJob(enemyTarget);
				return false;
			}
			bool num = CoverUtility.CalculateOverallBlockChance(__0, enemyTarget.Position, __0.Map) > 0.01f;
			bool flag = __0.Position.Standable(__0.Map) && __0.Map.pawnDestinationReservationManager.CanReserve(__0.Position, __0);
			bool flag2 = verb.CanHitTarget(enemyTarget);
			bool flag3 = (__0.Position - enemyTarget.Position).LengthHorizontalSquared < 25;
			if ((num && flag && flag2) || (flag3 && flag2))
			{
				__result = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_PokemonWaitCombat"), ExpiryInterval_ShooterSucceeded.RandomInRange, checkOverrideOnExpiry: true);
				return false;
			}
			if (!TryFindShootingPosition(__0, out var dest))
			{
				__result = null;
				return false;
			}
			if (dest == __0.Position)
			{
				__result = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_PokemonWaitCombat"), ExpiryInterval_ShooterSucceeded.RandomInRange, checkOverrideOnExpiry: true);
				return false;
			}
			Job job = JobMaker.MakeJob(JobDefOf.Goto, dest);
			job.expiryInterval = ExpiryInterval_ShooterSucceeded.RandomInRange;
			job.checkOverrideOnExpire = true;
			__result = job;
			return false;
		}

		private static bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
		{
			Thing enemyTarget = pawn.mindState.enemyTarget;
			Verb verb = pawn.TryGetAttackVerb(enemyTarget);
			if (verb == null)
			{
				dest = IntVec3.Invalid;
				return false;
			}
			CastPositionRequest newReq = default(CastPositionRequest);
			newReq.caster = pawn;
			newReq.target = enemyTarget;
			newReq.verb = verb;
			newReq.maxRangeFromTarget = verb.verbProps.range;
			newReq.wantCoverFromTarget = false;
			return CastPositionFinder.TryFindCastPosition(newReq, out dest);
		}
		private static bool ExtraTargetValidator(Pawn pawn, Thing target)
		{
			return true;
		}
		

		private static Job MeleeAttackJob(Thing enemyTarget)
		{
			Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, enemyTarget);
			job.expiryInterval = ExpiryInterval_Melee.RandomInRange;
			job.checkOverrideOnExpire = true;
			job.expireRequiresEnemiesNearby = true;
			return job;
		}

		private static void UpdateEnemyTarget(Pawn pawn)
		{
			Thing thing = pawn.mindState.enemyTarget;
			if (thing != null && (thing.Destroyed || Find.TickManager.TicksGame - pawn.mindState.lastEngageTargetTick > 400 || !pawn.CanReach(thing, PathEndMode.Touch, Danger.Deadly) || (float)(pawn.Position - thing.Position).LengthHorizontalSquared > targetKeepRadius * targetKeepRadius || ((IAttackTarget)thing).ThreatDisabled(pawn)))
			{
				thing = null;
			}
			if (thing == null)
			{
				thing = FindAttackTargetIfPossible(pawn);
				if (thing != null)
				{
					MethodInfo methodNotify_EngagedTarget = pawn.mindState.GetType().GetMethod("Notify_EngagedTarget", BindingFlags.NonPublic | BindingFlags.Instance);
					methodNotify_EngagedTarget.Invoke(pawn.mindState, new object[] {  });
					pawn.GetLord()?.Notify_PawnAcquiredTarget(pawn, thing);
				}
			}
			else
			{
				Thing thing2 = FindAttackTargetIfPossible(pawn);
				if (thing2 == null)
				{
                    thing = null;
				}
				else if (thing2 != null && thing2 != thing)
				{
					MethodInfo methodNotify_EngagedTarget = pawn.mindState.GetType().GetMethod("Notify_EngagedTarget", BindingFlags.NonPublic | BindingFlags.Instance);
					methodNotify_EngagedTarget.Invoke(pawn.mindState, new object[] { });
					thing = thing2;
				}
			}
			pawn.mindState.enemyTarget = thing;
			if (thing is Pawn && thing.Faction == Faction.OfPlayer && pawn.Position.InHorDistOf(thing.Position, 40f))
			{
				Find.TickManager.slower.SignalForceNormalSpeed();
			}
		}

		private static Thing FindAttackTargetIfPossible(Pawn pawn)
		{
			if (pawn.TryGetAttackVerb(null) == null)
			{
				return null;
			}
			return FindAttackTarget(pawn);
		}

		private static Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
			return (Thing)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, (Thing x) => ExtraTargetValidator(pawn, x), 0f, 9999);
		}
	}
}
