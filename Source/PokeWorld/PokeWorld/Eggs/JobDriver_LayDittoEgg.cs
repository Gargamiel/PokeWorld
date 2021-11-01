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
	public class JobDriver_LayDittoEgg : JobDriver
	{
		private const int LayEgg = 500;

		private const TargetIndex LaySpotInd = TargetIndex.A;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
			yield return Toils_General.Wait(500);
			yield return Toils_General.Do(delegate
			{
				Thing thing = pawn.GetComp<CompDittoEggLayer>().ProduceEgg();
				if(thing != null)
                {
					GenSpawn.Spawn(thing, pawn.Position, pawn.Map).SetForbiddenIfOutsideHomeArea();
				}		
			});
		}
	}
}
