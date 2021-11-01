using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI;

namespace PokeWorld
{
    public class StorageSystem : WorldComponent, IThingHolder
	{
		public ThingOwner innerContainer;
		protected bool contentsKnown;
		public readonly int maxCount = 999;
		public bool HasAnyContents => innerContainer.Count > 0;

		public StorageSystem(World world) : base(world)
		{
			innerContainer = new ThingOwner<Pawn>(this);
		}
		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}
		public List<Thing> ContainedThing
		{
			get
			{
				if (innerContainer.Count != 0)
				{
					return innerContainer.ToList();
				}
				return null;
			}
		}
		public virtual bool Accepts(CryptosleepBall cryptosleepBall)
		{
			if (cryptosleepBall.ContainedThing != null && cryptosleepBall.ContainedThing.GetType() == typeof(Pawn))
			{
				return true;
			}
			return false;
		}
		public virtual bool TryAcceptThing(CryptosleepBall cryptosleepBall, bool allowSpecialEffects = true)
		{
			if (!Accepts(cryptosleepBall))
			{
				return false;
			}
			bool flag = false;
			if (cryptosleepBall.ContainedThing != null)
			{
				cryptosleepBall.innerContainer.TryTransferToContainer(cryptosleepBall.ContainedThing, innerContainer, cryptosleepBall.stackCount);
				cryptosleepBall.Destroy();
				flag = true;
			}
			if (flag)
			{
				if (cryptosleepBall.Faction != null && cryptosleepBall.Faction.IsPlayer)
				{
					contentsKnown = true;
				}
				return true;
			}
			return false;
		}
		public static Building_PortableComputer FindPortableComputerFor(CryptosleepBall p, Pawn traveler, bool ignoreOtherReservations = false)
		{
			foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => typeof(Building_PortableComputer).IsAssignableFrom(def.thingClass)))
			{
				Building_PortableComputer building_PortableComputer = (Building_PortableComputer)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(item), PathEndMode.InteractionCell, TraverseParms.For(traveler), 9999f);
				if (building_PortableComputer != null && building_PortableComputer.TryGetComp<CompPowerTrader>().PowerOn && ReservationUtility.CanReserve(traveler, building_PortableComputer))
				{
					return building_PortableComputer;
				}
			}
			return null;
		}
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public IThingHolder ParentHolder
		{
			get
			{
				return null;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "PW_innerContainer");
			Scribe_Values.Look(ref contentsKnown, "PW_contentsKnown", defaultValue: false);
		}
	}
}
