using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace PokeWorld
{
    public class CryptosleepBall : ThingWithComps, ISuspendableThingHolder, IOpenable
    {
		public ThingOwner innerContainer;
		protected bool contentsKnown = false;
		public bool wantPutInPortableComputer = false;
		public int OpenTicks 
		{
            get
            {
				return 60;
			}			
		}

		public bool HasAnyContents => innerContainer.Count > 0;

		public bool IsContentsSuspended => true;

		public Thing ContainedThing
		{
			get
			{
				if (innerContainer.Count != 0)
				{
					return innerContainer[0];
				}
				return null;
			}
		}

		public bool CanOpen => HasAnyContents;

		public CryptosleepBall()
		{
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: true);
		}

        public override string Label
		{
            get
            {
				if(contentsKnown && ContainedThing is Pawn pawn)
                {
					return base.Label + " (" + pawn.Name + ")";
				}
                else
                {
					return base.Label;
				}
			}
		}
			

        public virtual bool Accepts(Thing thing)
		{
			return innerContainer.CanAcceptAnyOf(thing);
		}

		public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
		{
			if (!Accepts(thing))
			{
				return false;
			}
			bool flag = false;
			if (thing.holdingOwner != null)
			{
				thing.holdingOwner.TryTransferToContainer(thing, innerContainer, thing.stackCount);
				flag = true;
			}
			else
			{
				flag = innerContainer.TryAdd(thing);
			}
			if (flag)
			{
				if (thing.Faction != null && thing.Faction.IsPlayer)
				{
					contentsKnown = true;
				}
				return true;
			}
			return false;
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}
		public override void TickRare()
		{
			base.TickRare();
			innerContainer.ThingOwnerTickRare();
		}

		public override void Tick()
		{
			base.Tick();
			innerContainer.ThingOwnerTick();           
		}

		public virtual void Open()
		{
			if (HasAnyContents)
			{
				EjectContents();
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "PW_innerContainer", this);
			Scribe_Values.Look(ref contentsKnown, "PW_contentsKnown", defaultValue: false);
			Scribe_Values.Look(ref wantPutInPortableComputer, "PW_wantPutInPortableComputer", defaultValue: false);
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (base.Faction != null && base.Faction.IsPlayer)
			{
				contentsKnown = true;
			}
			if (innerContainer.NullOrEmpty())
			{
				Log.Error("Error: Tried to spawn empty Poké Ball");
				Destroy();
			}
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			if (innerContainer.Count > 0 &&  mode == DestroyMode.KillFinalize)
			{
				List<Pawn> list = new List<Pawn>();
				foreach (Thing item in (IEnumerable<Thing>)innerContainer)
				{
					Pawn pawn = item as Pawn;
					if (pawn != null)
					{
						list.Add(pawn);
					}
				}
				foreach (Pawn item2 in list)
				{
					HealthUtility.DamageUntilDowned(item2);
				}
				EjectContents();
			}
            else
            {
				innerContainer.ClearAndDestroyContents();
				base.Destroy(mode);
			}			
		}

		public virtual void EjectContents()
		{
			contentsKnown = true;
			IntVec3 pos = InteractionCell;
			Map map = Map;
			DeSpawn();
			innerContainer.TryDropAll(pos, map, ThingPlaceMode.Near);
			if (innerContainer.Count == 0)
			{
				Destroy();
			}						
		}

		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			string str;
			if (contentsKnown)
            {
				str = innerContainer.ContentsString;
			}
            else
            {
				str = "UnknownLower".Translate();
			}
			if (contentsKnown && innerContainer.Count > 0)
            {
				Pawn pokemon = innerContainer[0] as Pawn;
				if (pokemon != null)
                {
					CompPokemon comp = pokemon.TryGetComp<CompPokemon>();
					if (comp != null)
					{
						string species = pokemon.def.label;
						string gender = pokemon.gender != Gender.None ? pokemon.gender.ToString().ToLower() : null;
						string level = comp.levelTracker.level.ToString();
						if (!text.NullOrEmpty())
						{
							text += "\n";
						}
						if(gender != null)
                        {
							return text + "PW_PokeballContainsLongGendered".Translate(str.CapitalizeFirst(), level, gender, species);
						}
                        else
                        {
							return text + "PW_PokeballContainsLongNoGender".Translate(str.CapitalizeFirst(), level, species);
						}
					}
				}
            }		
			if (!text.NullOrEmpty())
			{
				text += "\n";
			}
			return text + "PW_PokeballContainsShort".Translate(str.CapitalizeFirst());
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo item in base.GetGizmos())
			{
				yield return item;
			}
			if (DefDatabase<ResearchProjectDef>.GetNamed("PW_StorageSystem").IsFinished && Faction == Faction.OfPlayer)
			{
				if (Map.designationManager.DesignationOn(this, DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer")) == null)
				{
					Command_Action command_PutInPC = new Command_Action();
					command_PutInPC.defaultLabel = "PW_DesignationStoreInPC".Translate();
					command_PutInPC.defaultDesc = "PW_DesignationStoreInPCDesc".Translate();
					command_PutInPC.hotKey = KeyBindingDefOf.Misc2;
					command_PutInPC.icon = ContentFinder<Texture2D>.Get("UI/Designators/PortableComputer");
					command_PutInPC.action = delegate ()
					{
						PutInPortableComputerUtility.UpdatePutInPortableComputerDesignation(this);
					};
					yield return command_PutInPC;
				}
			}
		}		
	}
}
