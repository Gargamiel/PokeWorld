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
	public class JobDriver_PokemonWaitCombat : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				base.Map.pawnDestinationReservationManager.Reserve(pawn, job, pawn.Position);
				pawn.pather.StopDead();
				CheckForAutoAttack();
			};
			toil.tickAction = delegate
			{
				if (job.expiryInterval == -1 && job.def == DefDatabase<JobDef>.GetNamed("PW_PokemonWaitCombat"))
				{
					Log.Error(string.Concat(pawn, " in eternal WaitCombat"));
					ReadyForNextToil();
				}
				if (pawn.Faction != null && pawn.Faction.IsPlayer && !PokemonMasterUtility.IsPokemonInMasterRange(pawn))
				{
					ReadyForNextToil();
				}
				else if ((Find.TickManager.TicksGame + pawn.thingIDNumber) % 4 == 0)
				{
					CheckForAutoAttack();
				}
			};
			DecorateWaitToil(toil);
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			if (pawn.mindState != null && pawn.mindState.duty != null && pawn.mindState.duty.focus != null)
			{
				LocalTargetInfo focusLocal = pawn.mindState.duty.focus;
				toil.handlingFacing = false;
				toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
				{
					pawn.rotationTracker.FaceTarget(focusLocal);
				});
			}
			else if (pawn.Faction != null && pawn.Faction.IsPlayer && PokemonMasterUtility.IsPokemonInMasterRange(pawn))
            {
				toil.handlingFacing = false;
				toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
				{
					pawn.Rotation = Rot4.South;
				});			
			}
			yield return toil;
		}

		public virtual void DecorateWaitToil(Toil wait)
		{
		}

		public override void Notify_StanceChanged()
		{
			if (pawn.stances.curStance is Stance_Mobile)
			{
				CheckForAutoAttack();
			}
		}

		private void CheckForAutoAttack()
		{
			if (base.pawn.Downed || base.pawn.stances.FullBodyBusy)
			{
				return;
			}
			collideWithPawns = false;
			Fire fire = null;
			for (int i = 0; i < 9; i++)
			{
				IntVec3 c = base.pawn.Position + GenAdj.AdjacentCellsAndInside[i];
				if (!c.InBounds(base.pawn.Map))
				{
					continue;
				}
				List<Thing> thingList = c.GetThingList(base.Map);
				for (int j = 0; j < thingList.Count; j++)
				{
					Pawn pawn = thingList[j] as Pawn;
					if (pawn != null && !pawn.Downed && base.pawn.HostileTo(pawn) && GenHostility.IsActiveThreatTo(pawn, base.pawn.Faction))
					{
						base.pawn.meleeVerbs.TryMeleeAttack(pawn);
						collideWithPawns = true;
						return;
					}					
					Fire fire2 = thingList[j] as Fire;
					if (fire2 != null && (fire == null || fire2.fireSize < fire.fireSize || i == 8) && (fire2.parent == null || fire2.parent != base.pawn))
					{
						fire = fire2;
					}					
				}
			}
			if (fire != null && (!base.pawn.InMentalState || base.pawn.MentalState.def.allowBeatfire))
			{
				base.pawn.natives.TryBeatFire(fire);
			}
			else
			{
				Verb currentEffectiveVerb = base.pawn.CurrentEffectiveVerb;
				if (currentEffectiveVerb != null && !(currentEffectiveVerb.tool != null))
				{
					TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToAll | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
					if (currentEffectiveVerb.IsIncendiary_Ranged())
					{
						targetScanFlags |= TargetScanFlags.NeedNonBurning;
					}
					Thing thing = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(base.pawn, targetScanFlags);
					if (thing != null)
					{
						base.pawn.TryStartAttack(thing);
						collideWithPawns = true;
					}
				}
			}
		}
	}
}
