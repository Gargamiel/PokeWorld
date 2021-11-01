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
	public class CompDittoEggLayer : ThingComp
	{
		private float eggProgress;
		private int fertilizationCount;
		private Pawn fertilizedBy;
		private float eggLayIntervalDays;
		private ThingDef eggFertilizedDef;

		private bool Active
		{
			get
			{
				return true;
			}
		}

		public bool CanLayNow
		{
			get
			{
				if (!Active)
				{
					return false;
				}
				return eggProgress >= 1f;
			}
		}

		public bool FullyFertilized => fertilizationCount >= Props.eggFertilizationCountMax;

		private bool ProgressStoppedBecauseUnfertilized
		{
			get
			{
				if (Props.eggProgressUnfertilizedMax < 1f && fertilizationCount == 0)
				{
					return eggProgress >= Props.eggProgressUnfertilizedMax;
				}
				return false;
			}
		}

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
			eggLayIntervalDays = Props.eggLayIntervalDays;
        }

        public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref eggProgress, "eggProgress", 0f);
			Scribe_Values.Look(ref fertilizationCount, "fertilizationCount", 0);
			Scribe_Values.Look(ref eggLayIntervalDays, "eggLayIntervalDays", 1);
			Scribe_Defs.Look(ref eggFertilizedDef, "eggFertilizedDef");
			Scribe_References.Look(ref fertilizedBy, "fertilizedBy", true);
		}

		public override void CompTick()
		{
			if (Active)
			{
				float num = 1f / (eggLayIntervalDays * 60000f);
				Pawn pawn = parent as Pawn;
				if (pawn != null)
				{
					num *= PawnUtility.BodyResourceGrowthSpeed(pawn);
				}
				eggProgress += num;
				if (eggProgress > 1f)
				{
					eggProgress = 1f;
				}
				if (ProgressStoppedBecauseUnfertilized)
				{
					eggProgress = Props.eggProgressUnfertilizedMax;
				}
			}
		}

		public void Fertilize(Pawn male)
		{
			fertilizationCount = Props.eggFertilizationCountMax;
			fertilizedBy = male;
			eggFertilizedDef = male.TryGetComp<CompEggLayer>().Props.eggFertilizedDef;
			eggLayIntervalDays = male.TryGetComp<CompEggLayer>().Props.eggLayIntervalDays;
		}

		public CompProperties_DittoEggLayer Props => (CompProperties_DittoEggLayer)props;

		public virtual Thing ProduceEgg()
		{
			if (!Active)
			{
				Log.Error("LayEgg while not Active: " + parent);
			}
			eggProgress = 0f;
			int randomInRange = Props.eggCountRange.RandomInRange;
			if (randomInRange == 0)
			{
				return null;
			}
			Thing thing = null;
			if (fertilizationCount > 0 && eggFertilizedDef != null)
			{
				thing = ThingMaker.MakeThing(eggFertilizedDef);				
			}
			else if(Props.eggUnfertilizedDef != null)
			{
				thing = ThingMaker.MakeThing(Props.eggUnfertilizedDef);
			}
			fertilizationCount = Mathf.Max(0, fertilizationCount - randomInRange);
			if(thing == null)
            {
				return thing;
			}		
			thing.stackCount = randomInRange;
			CompHatcher compHatcher = thing.TryGetComp<CompHatcher>();
			if (compHatcher != null)
			{
				compHatcher.hatcheeFaction = parent.Faction;
				Pawn pawn = parent as Pawn;
				if (pawn != null)
				{
					compHatcher.hatcheeParent = pawn;
				}
				if (fertilizedBy != null)
				{
					compHatcher.otherParent = fertilizedBy;
				}
			}
			eggLayIntervalDays = Props.eggLayIntervalDays;
			return thing;
		}
		public override string CompInspectStringExtra()
		{
			if (!Active)
			{
				return null;
			}
			string text = "EggProgress".Translate() + ": " + eggProgress.ToStringPercent();
			if (fertilizationCount > 0)
			{
				text += "\n" + "Fertilized".Translate();
			}
			else if (ProgressStoppedBecauseUnfertilized)
			{
				text += "\n" + "ProgressStoppedUntilFertilized".Translate();
			}
			return text;
		}
	}

	public class CompProperties_DittoEggLayer : CompProperties
	{
		public float eggLayIntervalDays = 1f;

		public IntRange eggCountRange = IntRange.one;

		public ThingDef eggUnfertilizedDef;

		public int eggFertilizationCountMax = 1;

		public float eggProgressUnfertilizedMax = 1f;

		public CompProperties_DittoEggLayer()
		{
			compClass = typeof(CompDittoEggLayer);
		}
	}
}
