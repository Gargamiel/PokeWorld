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
	public class Verb_PokemonMeleeMove : Verb_MeleeAttack
	{		
		public override bool Available()
		{
			CompPokemon comp = ((Pawn)caster).TryGetComp<CompPokemon>();
			if (comp != null)
			{
				MoveDef moveDef = comp.moveTracker.unlockableMoves.Keys.Where((MoveDef x) => x.tool == tool).First();
				return PokemonAttackGizmoUtility.ShouldUseMove((Pawn)caster, moveDef);
			}
			return false;
		}

		private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
		{
			float num = verbProps.AdjustedMeleeDamageAmount(this, CasterPawn);
			float armorPenetration = verbProps.AdjustedArmorPenetration(this, CasterPawn);
			DamageDef def = verbProps.meleeDamageDef;
			BodyPartGroupDef bodyPartGroupDef = null;
			HediffDef hediffDef = null;
			
			if (CasterIsPawn)
			{
				bodyPartGroupDef = verbProps.AdjustedLinkedBodyPartsGroup(tool);
				if (num >= 1f)
				{
					if (base.HediffCompSource != null)
					{
						hediffDef = base.HediffCompSource.Def;
					}
				}
				else
				{
					num = 1f;
					def = DamageDefOf.Blunt;
				}
			}
			ThingDef source = ((base.EquipmentSource == null) ? CasterPawn.def : base.EquipmentSource.def);
			Vector3 direction = (target.Thing.Position - CasterPawn.Position).ToVector3();
			DamageInfo damageInfo = new DamageInfo(def, num, armorPenetration, -1f, caster, null, source);
			damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
			damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
			damageInfo.SetWeaponHediff(hediffDef);
			damageInfo.SetAngle(direction);
			yield return damageInfo;
			if (tool != null && tool.extraMeleeDamages != null)
			{
				foreach (ExtraDamage extraMeleeDamage in tool.extraMeleeDamages)
				{
					if (Rand.Chance(extraMeleeDamage.chance))
					{
						num = extraMeleeDamage.amount;
						num = Rand.Range(num * 0.8f, num * 1.2f);
						damageInfo = new DamageInfo(extraMeleeDamage.def, num, extraMeleeDamage.AdjustedArmorPenetration(this, CasterPawn), -1f, caster, null, source);
						damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
						damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
						damageInfo.SetWeaponHediff(hediffDef);
						damageInfo.SetAngle(direction);
						yield return damageInfo;
					}
				}
			}
			if (!surpriseAttack || ((verbProps.surpriseAttack == null || verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty()) && (tool == null || tool.surpriseAttack == null || tool.surpriseAttack.extraMeleeDamages.NullOrEmpty())))
			{
				yield break;
			}
			IEnumerable<ExtraDamage> enumerable = Enumerable.Empty<ExtraDamage>();
			if (verbProps.surpriseAttack != null && verbProps.surpriseAttack.extraMeleeDamages != null)
			{
				enumerable = enumerable.Concat(verbProps.surpriseAttack.extraMeleeDamages);
			}
			if (tool != null && tool.surpriseAttack != null && !tool.surpriseAttack.extraMeleeDamages.NullOrEmpty())
			{
				enumerable = enumerable.Concat(tool.surpriseAttack.extraMeleeDamages);
			}
			foreach (ExtraDamage item in enumerable)
			{
				int num2 = GenMath.RoundRandom(item.AdjustedDamageAmount(this, CasterPawn));
				float armorPenetration2 = item.AdjustedArmorPenetration(this, CasterPawn);
				DamageInfo damageInfo2 = new DamageInfo(item.def, num2, armorPenetration2, -1f, caster, null, source);
				damageInfo2.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
				damageInfo2.SetWeaponBodyPartGroup(bodyPartGroupDef);
				damageInfo2.SetWeaponHediff(hediffDef);
				damageInfo2.SetAngle(direction);
				yield return damageInfo2;
			}
		}

		protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
		{
			DamageWorker.DamageResult result = new DamageWorker.DamageResult();
			foreach (DamageInfo item in DamageInfosToApply(target))
			{
				if (!target.ThingDestroyed)
				{
					CompPokemon comp = caster.TryGetComp<CompPokemon>();
					if(comp != null)
                    {
						comp.moveTracker.lastUsedMove = comp.moveTracker.unlockableMoves.Keys.Where((MoveDef x) => x.tool == tool).First();
                    }
					result = target.Thing.TakeDamage(item);
					continue;
				}
				return result;
			}
			return result;
		}
	}

}