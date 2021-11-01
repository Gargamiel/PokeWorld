using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace PokeWorld
{
	public class JobDriver_PokemonAttackMelee : JobDriver
	{
		private bool startedIncapacitated;

		private int numMeleeAttacksMade;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref startedIncapacitated, "startedIncapacitated", defaultValue: false);
			Scribe_Values.Look(ref numMeleeAttacksMade, "numMeleeAttacksMade", 0);
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			IAttackTarget attackTarget = job.targetA.Thing as IAttackTarget;
			if (attackTarget != null)
			{
				pawn.Map.attackTargetReservationManager.Reserve(pawn, job, attackTarget);
			}
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_General.DoAtomic(delegate
			{
				Pawn pawn = job.targetA.Thing as Pawn;
				if (pawn != null)
                {
					if (pawn.Downed && base.pawn.mindState.duty != null && base.pawn.mindState.duty.attackDownedIfStarving && base.pawn.Starving())
					{
						job.killIncappedTarget = true;
					}
					startedIncapacitated = pawn.Downed;
				}						
			});
			yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
			yield return Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, delegate
			{
				Thing thing = job.GetTarget(TargetIndex.A).Thing;
				Pawn p;
				if (job.reactingToMeleeThreat && (p = thing as Pawn) != null && !p.Awake())
				{
					EndJobWith(JobCondition.InterruptForced);
				}
				if (pawn.Faction != null && pawn.Faction.IsPlayer && !PokemonMasterUtility.IsPokemonInMasterRange(pawn))
				{
					EndJobWith(JobCondition.Succeeded);
				}
				else if (pawn.meleeVerbs.TryMeleeAttack(thing, job.verbToUse) && pawn.CurJob != null && pawn.jobs.curDriver == this)
				{
					numMeleeAttacksMade++;
					if (base.TargetA.Thing.Destroyed || ((p = thing as Pawn) != null && !startedIncapacitated && p.Downed) || (p != null && p.IsInvisible()))
					{
						EndJobWith(JobCondition.Succeeded);
						if (pawn.Faction != null && pawn.Faction.IsPlayer && PokemonMasterUtility.IsPokemonInMasterRange(pawn))
						{
							if (pawn.jobs.jobQueue.Count == 0)
							{
								Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_PokemonWaitCombat"));
								job.expiryInterval = 0;
								pawn.jobs.TryTakeOrderedJob(job);
							}
						}
						return;
					}
					else if (numMeleeAttacksMade >= job.maxNumMeleeAttacks)
					{
						EndJobWith(JobCondition.Succeeded);
					}
				}
			}).FailOnDespawnedOrNull(TargetIndex.A);
		}

		public override void Notify_PatherFailed()
		{
			if (job.attackDoorIfTargetLost)
			{
				Thing thing;
				using (PawnPath pawnPath = base.Map.pathFinder.FindPath(pawn.Position, base.TargetA.Cell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors)))
				{
					if (!pawnPath.Found)
					{
						return;
					}
					thing = pawnPath.FirstBlockingBuilding(out var _, pawn);
				}
				if (thing != null && thing.Position.InHorDistOf(pawn.Position, 6f))
				{
					job.targetA = thing;
					job.maxNumMeleeAttacks = Rand.RangeInclusive(2, 5);
					job.expiryInterval = Rand.Range(2000, 4000);
					return;
				}
			}
			base.Notify_PatherFailed();
		}

		public override bool IsContinuation(Job j)
		{
			return job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
		}
	}
}
