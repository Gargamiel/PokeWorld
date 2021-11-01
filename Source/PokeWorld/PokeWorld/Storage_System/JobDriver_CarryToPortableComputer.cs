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
	public class JobDriver_CarryToPortableComputer : JobDriver
	{
		private const TargetIndex TakeeInd = TargetIndex.A;

		private const TargetIndex DropPodInd = TargetIndex.B;

		StorageSystem storageSystem = Find.World.GetComponent<StorageSystem>();

		protected CryptosleepBall Takee => (CryptosleepBall)job.GetTarget(TargetIndex.A).Thing;

		protected Building_PortableComputer DropPod => (Building_PortableComputer)job.GetTarget(TargetIndex.B).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Takee, job, 1, 1, null, errorOnFailed))
			{
				return pawn.Reserve(DropPod, job, 1, 1, null, errorOnFailed);
			}
			return false;
		}
		
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnDestroyedOrNull(TargetIndex.B);
			this.FailOn(() => !DropPod.TryGetComp<CompPowerTrader>().PowerOn);
			this.FailOnThingMissingDesignation(TargetIndex.A, DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer"));
			this.FailOn(() => !storageSystem.Accepts(Takee));
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.B)
				.FailOn(() => storageSystem.GetDirectlyHeldThings().Count >= storageSystem.maxCount)
				.FailOn(() => Takee.ContainedThing == null)
				.FailOn(() => !pawn.CanReach(Takee, PathEndMode.OnCell, Danger.Deadly))
				.FailOn(() => !DropPod.TryGetComp<CompPowerTrader>().PowerOn)
				.FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			job.count = 1;
			yield return Toils_Haul.StartCarryThing(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell)
				.FailOn(() => !DropPod.TryGetComp<CompPowerTrader>().PowerOn);
			Toil toil = Toils_General.Wait(100, TargetIndex.B);
			toil.FailOnCannotTouch(TargetIndex.B, PathEndMode.InteractionCell);
			toil.FailOn(() => !DropPod.TryGetComp<CompPowerTrader>().PowerOn);
			toil.WithProgressBarToilDelay(TargetIndex.B);
			yield return toil;
			Toil toil2 = new Toil();
			toil2.initAction = delegate
			{
				storageSystem.TryAcceptThing(Takee);
			};
			toil2.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return toil2;
		}
		
		public override object[] TaleParameters()
		{
			return new object[2]
			{
			pawn,
			Takee.ContainedThing
			};
		}
	}
}
