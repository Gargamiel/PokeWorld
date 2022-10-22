using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace PokeWorld
{
	public struct PokeBallShotReport
	{
		private TargetInfo target;

		private float distance;

		private List<CoverInfo> covers;

		private float coversOverallBlockChance;

		private float factorFromShooterAndDist;

		private float factorFromEquipment;

		private float factorFromTargetSize;

		private float factorFromWeather;

		private float forcedMissRadius;

		private float offsetFromDarkness;

		private float factorFromCoveringGas;

		private ShootLine shootLine;

		private float FactorFromPosture
		{
			get
			{
				if (target.HasThing && target.Thing is Pawn p && distance >= 4.5f && p.GetPosture() != 0)
				{
					return 0.2f;
				}
				return 1f;
			}
		}

		private float FactorFromExecution
		{
			get
			{
				if (target.HasThing && target.Thing is Pawn p && distance <= 3.9f && p.GetPosture() != 0)
				{
					return 7.5f;
				}
				return 1f;
			}
		}

		public float AimOnTargetChance_StandardTarget
		{
			get
			{
				float num = factorFromShooterAndDist * factorFromEquipment * factorFromWeather * factorFromCoveringGas * FactorFromExecution;
				num += offsetFromDarkness;
				if (num < 0.0201f)
				{
					num = 0.0201f;
				}
				return num;
			}
		}

		public float AimOnTargetChance_IgnoringPosture => AimOnTargetChance_StandardTarget * factorFromTargetSize;

		public float AimOnTargetChance => AimOnTargetChance_IgnoringPosture * FactorFromPosture;

		public float PassCoverChance => 1f - coversOverallBlockChance;

		public float TotalEstimatedHitChance => Mathf.Clamp01(AimOnTargetChance * PassCoverChance);

		public ShootLine ShootLine => shootLine;

		public static PokeBallShotReport HitReportFor(Thing caster, Verb verb, LocalTargetInfo target)
		{
			IntVec3 cell = target.Cell;
			PokeBallShotReport result = default(PokeBallShotReport);
			result.distance = (cell - caster.Position).LengthHorizontal;
			result.target = target.ToTargetInfo(caster.Map);
			result.factorFromShooterAndDist = HitFactorFromShooter(caster, result.distance);
			result.factorFromEquipment = verb.verbProps.GetHitChanceFactor(verb.EquipmentSource, result.distance);
			result.covers = CoverUtility.CalculateCoverGiverSet(target, caster.Position, caster.Map);
			result.coversOverallBlockChance = CoverUtility.CalculateOverallBlockChance(target, caster.Position, caster.Map);
			result.factorFromCoveringGas = 1f;
			if (verb.TryFindShootLineFromTo(verb.caster.Position, target, out result.shootLine))
			{
				foreach (IntVec3 item in result.shootLine.Points())
				{
					if (item.AnyGas(caster.Map, GasType.BlindSmoke))
					{
						result.factorFromCoveringGas = 0.7f;
					}
				}
			}
			else
			{
				result.shootLine = new ShootLine(IntVec3.Invalid, IntVec3.Invalid);
			}
			if (!caster.Position.Roofed(caster.Map) || !target.Cell.Roofed(caster.Map))
			{
				result.factorFromWeather = caster.Map.weatherManager.CurWeatherAccuracyMultiplier;
			}
			else
			{
				result.factorFromWeather = 1f;
			}
			if (target.HasThing)
			{
				if (target.Thing is Pawn pawn)
				{
					result.factorFromTargetSize = pawn.BodySize;
				}
				else
				{
					result.factorFromTargetSize = target.Thing.def.fillPercent * (float)target.Thing.def.size.x * (float)target.Thing.def.size.z * 2.5f;
				}
				result.factorFromTargetSize = Mathf.Clamp(result.factorFromTargetSize, 0.5f, 2f);
			}
			else
			{
				result.factorFromTargetSize = 1f;
			}
			result.forcedMissRadius = verb.verbProps.ForcedMissRadius;
			result.offsetFromDarkness = 0f;
			if (ModsConfig.IdeologyActive && target.HasThing)
			{
				if (DarknessCombatUtility.IsOutdoorsAndLit(target.Thing))
				{
					result.offsetFromDarkness = caster.GetStatValue(StatDefOf.ShootingAccuracyOutdoorsLitOffset);
				}
				else if (DarknessCombatUtility.IsOutdoorsAndDark(target.Thing))
				{
					result.offsetFromDarkness = caster.GetStatValue(StatDefOf.ShootingAccuracyOutdoorsDarkOffset);
				}
				else if (DarknessCombatUtility.IsIndoorsAndDark(target.Thing))
				{
					result.offsetFromDarkness = caster.GetStatValue(StatDefOf.ShootingAccuracyIndoorsDarkOffset);
				}
				else if (DarknessCombatUtility.IsIndoorsAndLit(target.Thing))
				{
					result.offsetFromDarkness = caster.GetStatValue(StatDefOf.ShootingAccuracyIndoorsLitOffset);
				}
			}
			return result;
		}

		public static float HitFactorFromShooter(Thing caster, float distance)
		{
			return HitFactorFromShooter((caster is Pawn) ? caster.GetStatValue(DefDatabase<StatDef>.GetNamed("PW_PokeBallAccuracyPawn")) : caster.GetStatValue(StatDefOf.ShootingAccuracyTurret), distance);
		}

		public static float HitFactorFromShooter(float accRating, float distance)
		{
			return Mathf.Max(Mathf.Pow(accRating, distance), 0.0201f);
		}

		public string GetTextReadout()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (forcedMissRadius > 0.5f)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("WeaponMissRadius".Translate() + ": " + forcedMissRadius.ToString("F1"));
				stringBuilder.AppendLine("DirectHitChance".Translate() + ": " + (1f / (float)GenRadial.NumCellsInRadius(forcedMissRadius)).ToStringPercent());
			}
			else
			{
				stringBuilder.AppendLine(TotalEstimatedHitChance.ToStringPercent());
				stringBuilder.AppendLine("   " + "ShootReportShooterAbility".Translate() + ": " + factorFromShooterAndDist.ToStringPercent());
				stringBuilder.AppendLine("   " + "ShootReportWeapon".Translate() + ": " + factorFromEquipment.ToStringPercent());
				if (target.HasThing && factorFromTargetSize != 1f)
				{
					stringBuilder.AppendLine("   " + "TargetSize".Translate() + ": " + factorFromTargetSize.ToStringPercent());
				}
				if (factorFromWeather < 0.99f)
				{
					stringBuilder.AppendLine("   " + "Weather".Translate() + ": " + factorFromWeather.ToStringPercent());
				}
				if (factorFromCoveringGas < 0.99f)
				{
					stringBuilder.AppendLine("   " + "BlindSmoke".Translate().CapitalizeFirst() + ": " + factorFromCoveringGas.ToStringPercent());
				}
				if (FactorFromPosture < 0.9999f)
				{
					stringBuilder.AppendLine("   " + "TargetProne".Translate() + ": " + FactorFromPosture.ToStringPercent());
				}
				if (FactorFromExecution != 1f)
				{
					stringBuilder.AppendLine("   " + "Execution".Translate() + ": " + FactorFromExecution.ToStringPercent());
				}
				if (ModsConfig.IdeologyActive && target.HasThing && offsetFromDarkness != 0f)
				{
					if (DarknessCombatUtility.IsOutdoorsAndLit(target.Thing))
					{
						stringBuilder.AppendLine("   " + StatDefOf.ShootingAccuracyOutdoorsLitOffset.LabelCap + ": " + offsetFromDarkness.ToStringPercent());
					}
					else if (DarknessCombatUtility.IsOutdoorsAndDark(target.Thing))
					{
						stringBuilder.AppendLine("   " + StatDefOf.ShootingAccuracyOutdoorsDarkOffset.LabelCap + ": " + offsetFromDarkness.ToStringPercent());
					}
					else if (DarknessCombatUtility.IsIndoorsAndDark(target.Thing))
					{
						stringBuilder.AppendLine("   " + StatDefOf.ShootingAccuracyIndoorsDarkOffset.LabelCap + ": " + offsetFromDarkness.ToStringPercent());
					}
					else if (DarknessCombatUtility.IsIndoorsAndLit(target.Thing))
					{
						stringBuilder.AppendLine("   " + StatDefOf.ShootingAccuracyIndoorsLitOffset.LabelCap + "   " + offsetFromDarkness.ToStringPercent());
					}
				}
				if (PassCoverChance < 1f)
				{
					stringBuilder.AppendLine("   " + "ShootingCover".Translate() + ": " + PassCoverChance.ToStringPercent());
					for (int i = 0; i < covers.Count; i++)
					{
						CoverInfo coverInfo = covers[i];
						if (coverInfo.BlockChance > 0f)
						{
							stringBuilder.AppendLine("     " + "CoverThingBlocksPercentOfShots".Translate(coverInfo.Thing.LabelCap, coverInfo.BlockChance.ToStringPercent(), new NamedArgument(coverInfo.Thing.def, "COVER")).CapitalizeFirst());
						}
					}
				}
				else
				{
					stringBuilder.AppendLine("   (" + "NoCoverLower".Translate() + ")");
				}
			}
			return stringBuilder.ToString();
		}

		public Thing GetRandomCoverToMissInto()
		{
			if (covers.TryRandomElementByWeight((CoverInfo c) => c.BlockChance, out var result))
			{
				return result.Thing;
			}
			return null;
		}
	}
}