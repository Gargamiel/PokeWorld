using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace PokeWorld
{
    class Designator_PutInPortableComputer : Designator
	{
		private List<CryptosleepBall> justDesignated = new List<CryptosleepBall>();

		public override int DraggableDimensions => 2;

		protected override DesignationDef Designation => DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer");

		public Designator_PutInPortableComputer()
		{
			defaultLabel = "PW_DesignationStoreInPC".Translate();
			defaultDesc = "PW_DesignationStoreInPCDesc".Translate();
			icon = ContentFinder<Texture2D>.Get("UI/Designators/PortableComputer");
			soundDragSustain = SoundDefOf.Designate_DragStandard;
			soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			useMouseIcon = true;
			soundSucceeded = SoundDefOf.Designate_Hunt;
			hotKey = KeyBindingDefOf.Misc7;
		}

        public override bool Visible => DefDatabase<ResearchProjectDef>.GetNamed("PW_StorageSystem").IsFinished;

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds(base.Map))
			{
				return false;
			}
			if (!PCStorableInCell(c).Any())
			{
				return "PW_DesignationStoreInPCWarning".Translate();
			}
			return true;
		}

		public override void DesignateSingleCell(IntVec3 loc)
		{
			foreach (CryptosleepBall item in PCStorableInCell(loc))
			{
				DesignateThing(item);
			}
		}

		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			CryptosleepBall ball = t as CryptosleepBall;
			if (ball != null && ball.Faction == Faction.OfPlayer && base.Map.designationManager.DesignationOn(ball, Designation) == null)
			{
				return true;
			}
			return false;
		}

		public override void DesignateThing(Thing t)
		{
			base.Map.designationManager.AddDesignation(new Designation(t, Designation));
			justDesignated.Add((CryptosleepBall)t);
		}

		protected override void FinalizeDesignationSucceeded()
		{
			base.FinalizeDesignationSucceeded();
			justDesignated.Clear();
		}

		private IEnumerable<CryptosleepBall> PCStorableInCell(IntVec3 c)
		{
			if (c.Fogged(base.Map))
			{
				yield break;
			}
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (CanDesignateThing(thingList[i]).Accepted)
				{
					yield return (CryptosleepBall)thingList[i];
				}
			}
		}
	}
}
