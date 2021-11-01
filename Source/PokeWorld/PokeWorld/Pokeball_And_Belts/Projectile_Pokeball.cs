using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;
using RimWorld;
using UnityEngine;


namespace PokeWorld
{
    public class Projectile_Pokeball : Projectile
	{
		private int ticksToDetonation;
		private Sustainer ambientSustainer;
		public Thing equipment;
		public float bonusBall;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToDetonation, "PW_ticksToDetonation", 0);
			Scribe_References.Look(ref equipment, "PW_equipment");
			Scribe_Values.Look(ref bonusBall, "PW_bonusBall", 0);
		}

		public override void Tick()
		{
			base.Tick();
			if (ticksToDetonation > 0)
			{
				ticksToDetonation--;
				if (ticksToDetonation <= 0)
				{
					Explode();
				}
			}
		}

		protected override void Impact(Thing hitThing)
		{
			if (def.projectile.explosionDelay == 0)
			{
				Explode();
				return;
			}
			landed = true;
			ticksToDetonation = def.projectile.explosionDelay;
		}
		public void Launch(Thing launcher, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null)
		{
			Launch(launcher, base.Position.ToVector3Shifted(), usedTarget, intendedTarget, hitFlags, equipment);
		}

		public void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment = null, ThingDef targetCoverDef = null)
		{
			this.launcher = launcher;
			this.origin = origin;
			this.usedTarget = usedTarget;
			this.intendedTarget = intendedTarget;
			this.targetCoverDef = targetCoverDef;
			HitFlags = hitFlags;
			if (equipment != null)
			{
				this.equipment = equipment;
				bonusBall = this.equipment.GetStatValue(DefDatabase<StatDef>.GetNamed("PW_BonusBall"));
				equipmentDef = equipment.def;
				weaponDamageMultiplier = equipment.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier);
			}
			else
			{
				equipmentDef = null;
				weaponDamageMultiplier = 1f;
			}
			destination = usedTarget.Cell.ToVector3Shifted() + Gen.RandomHorizontalVector(0.3f);
			ticksToImpact = Mathf.CeilToInt(StartingTicksToImpact);
			if (ticksToImpact < 1)
			{
				ticksToImpact = 1;
			}
			if (!def.projectile.soundAmbient.NullOrUndefined())
			{
				SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
				ambientSustainer = def.projectile.soundAmbient.TrySpawnSustainer(info);
			}
		}

		protected virtual void Explode()
		{
			Map map = base.Map;
			Destroy();
			if (base.def.projectile.explosionEffect != null)
			{
				Effecter effecter = base.def.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(base.Position, map), new TargetInfo(base.Position, map));
				effecter.Cleanup();
			}
			GenPokeBallExplosion.DoExplosion(base.Position, map, base.def.projectile.explosionRadius, base.def.projectile.damageDef, base.launcher, bonusBall, base.DamageAmount, base.ArmorPenetration, base.def.projectile.soundExplode, base.equipmentDef, base.def, intendedTarget.Thing, base.def.projectile.postExplosionSpawnThingDef, base.def.projectile.postExplosionSpawnChance, base.def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: base.def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: base.def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: base.def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: base.def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: base.def.projectile.explosionChanceToStartFire, damageFalloff: base.def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
		}
	}
}
