using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PokeWorld
{
	public class PokemonMoveProjectile : Projectile
	{
		protected MoveDef moveDef;
		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			Map map = base.Map;
			IntVec3 position = base.Position;
			base.Impact(hitThing);
			moveDef = ((Pawn)launcher).TryGetComp<CompPokemon>().moveTracker.lastUsedMove;
			BattleLogEntry_PokemonRangedMoveImpact battleLogEntry_RangedImpact = new BattleLogEntry_PokemonRangedMoveImpact(launcher, hitThing, intendedTarget.Thing, moveDef, def, targetCoverDef);
			Find.BattleLog.Add(battleLogEntry_RangedImpact);
			NotifyImpact(hitThing, map, position);
			if (hitThing != null)
			{
				DamageInfo dinfo = new DamageInfo(def.projectile.damageDef, base.DamageAmount, base.ArmorPenetration, ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
				hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
				Pawn pawn = hitThing as Pawn;
				if (pawn != null && pawn.stances != null && pawn.BodySize <= def.projectile.StoppingPower + 0.001f)
				{
					pawn.stances.stagger.StaggerFor(95);
				}
				if (def.projectile.extraDamages == null)
				{
					return;
				}
				foreach (ExtraDamage extraDamage in def.projectile.extraDamages)
				{
					if (Rand.Chance(extraDamage.chance))
					{
						DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
						hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
					}
				}
			}
			else
			{
				SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map));
				if (base.Position.GetTerrain(map).takeSplashes)
				{
					FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(base.DamageAmount) * 1f, 4f);
				}
				else
				{
					FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
				}
			}
		}

		private void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
		{
			BulletImpactData bulletImpactData = default(BulletImpactData);
			bulletImpactData.bullet = this as Projectile as Bullet;
			bulletImpactData.hitThing = hitThing;
			bulletImpactData.impactPosition = position;
			BulletImpactData impactData = bulletImpactData;
			hitThing?.Notify_BulletImpactNearby(impactData);
			int num = 9;
			for (int i = 0; i < num; i++)
			{
				IntVec3 c = position + GenRadial.RadialPattern[i];
				if (!c.InBounds(map))
				{
					continue;
				}
				List<Thing> thingList = c.GetThingList(map);
				for (int j = 0; j < thingList.Count; j++)
				{
					if (thingList[j] != hitThing)
					{
						thingList[j].Notify_BulletImpactNearby(impactData);
					}
				}
			}
		}
	}
}
