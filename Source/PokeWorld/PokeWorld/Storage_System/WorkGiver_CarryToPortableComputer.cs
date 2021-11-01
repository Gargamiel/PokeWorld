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
    class WorkGiver_CarryToPortableComputer : WorkGiver_Scanner
    {
		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<Designation> desList = pawn.Map.designationManager.allDesignations;
			for (int i = 0; i < desList.Count; i++)
			{
				if (desList[i].def == DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer"))
				{
					yield return desList[i].target.Thing;
				}
			}
		}

		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer"));
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (pawn.Map.designationManager.DesignationOn(t, DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer")) == null)
			{
				return false;
			}
			if (!pawn.CanReserve(t, 1, 1, null, forced))
			{
				return false;
			}
			if (StorageSystem.FindPortableComputerFor(t as CryptosleepBall, pawn) == null)
			{
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_CarryToPortableComputer"), t, StorageSystem.FindPortableComputerFor(t as CryptosleepBall, pawn));
		}
	}
}
