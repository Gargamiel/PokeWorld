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
    class JobDriver_PutInBall : JobDriver
    {
		protected Pawn pokemon => (Pawn)job.targetA.Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(pokemon, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOnAggroMentalState(TargetIndex.A);
			this.FailOnThingMissingDesignation(TargetIndex.A, DefDatabase<DesignationDef>.GetNamed("PW_PutInBall"));
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 100, useProgressBar: true);
			Toil enter = new Toil();
			enter.initAction = delegate
			{
				PutInBallUtility.PutInBall(pokemon);				
			};
			enter.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return enter;
		}
	}
}
