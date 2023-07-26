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
    class JobDriver_Fish : JobDriver
	{
		protected IntVec3 targetCell => job.GetTarget(TargetIndex.A).Cell;
		protected Thing fishingRod => job.GetTarget(TargetIndex.B).Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(targetCell, job, 1, 1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOn(() => !FishingUtility.IsFishingTerrain(targetCell.GetTerrain(pawn.Map)));
			this.FailOn(() => !pawn.EquippedWornOrInventoryThings.Contains(fishingRod));
			Toil gotoToil = Toils_Goto.GotoCell(targetCell, PathEndMode.Touch)
				.FailOn(() => !pawn.CanReach(targetCell, PathEndMode.Touch, Danger.Deadly));
			yield return gotoToil;
            int ticks = FishingUtility.ComputeFishingTicks(pawn, fishingRod);
            Toil toil = Toils_General.Wait(ticks, TargetIndex.A);
            toil.activeSkill = () => SkillDefOf.Animals;
            toil.WithProgressBarToilDelay(TargetIndex.A);
            yield return toil;
            Toil toil2 = new Toil();
            toil2.initAction = delegate
            {
                FishingUtility.TryFish(pawn, fishingRod, targetCell);
                //fishingSpot.Fish(pawn,rod,targetTerrain);
            };
            toil2.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return toil2;
            yield return Toils_Jump.Jump(gotoToil);
            /*Toil work = ToilMaker.MakeToil("MakeNewToils");
			work.tickAction = delegate
			{
				Pawn actor = work.actor;
				actor.skills.Learn(SkillDefOf.Animals, 0.035f);
			};
			work.defaultCompleteMode = ToilCompleteMode.Never;
			work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
			work.activeSkill = () => SkillDefOf.Animals;
			yield return work;*/
        }
	}
}
