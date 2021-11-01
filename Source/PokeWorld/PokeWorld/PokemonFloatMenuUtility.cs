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
    public static class PokemonFloatMenuUtility
    {
		
		public static Action GetRangedAttackAction(Pawn pawn, LocalTargetInfo target, out string failStr)
		{
			CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			failStr = "";
			if (comp == null || comp.moveTracker == null)
            {
				return null;
			}
            if (!PokemonAttackGizmoUtility.CanUseAnyRangedVerb(pawn))
            {
				return null;
            }
			Verb longestRangeVerb = PokemonAttackGizmoUtility.GetLongestRangeVerb(pawn);
			if (longestRangeVerb == null)
			{
				return null;
			}
			/*
			else if (!pawn.IsColonistPlayerControlled)
			{
				failStr = "CannotOrderNonControlledLower".Translate();
			}
			*/
			else if (target.IsValid && !longestRangeVerb.CanHitTarget(target))
			{
				if (!pawn.Position.InHorDistOf(target.Cell, longestRangeVerb.verbProps.range))
				{
					failStr = "OutOfRange".Translate();
				}
				float num = longestRangeVerb.verbProps.EffectiveMinRange(target, pawn);
				if ((float)pawn.Position.DistanceToSquared(target.Cell) < num * num)
				{
					failStr = "TooClose".Translate();
				}
				else
				{
					failStr = "CannotHitTarget".Translate();
				}
			}
			else if (pawn == target.Thing)
			{
				failStr = "CannotAttackSelf".Translate();
			}
			else if (pawn.playerSettings.Master == null || pawn.playerSettings.Master.Map != pawn.Map)
			{
				failStr = "This Pokémon has no master";
			}
			else if (pawn.playerSettings.Master.Drafted == false)
			{
				failStr = "This Pokémon master is not drafted";
			}
			else if (IntVec3Utility.DistanceTo(pawn.Position, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
			{
				failStr = "This Pokémon's master is too far";
			}
			/*
			else if (IntVec3Utility.DistanceTo(target.Cell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
			{
				failStr = "The target is too far from this Pokémon's master";
			}
			*/
			else
			{
				Pawn target2;
				if ((target2 = target.Thing as Pawn) == null || (!pawn.InSameExtraFaction(target2, ExtraFactionType.HomeFaction) && !pawn.InSameExtraFaction(target2, ExtraFactionType.MiniFaction)))
				{
					return delegate
					{
						Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_PokemonAttackStatic"), target);
						pawn.jobs.TryTakeOrderedJob(job);
					};
				}
				failStr = "CannotAttackSameFactionMember".Translate();
			}
			failStr = failStr.CapitalizeFirst();
			return null;
		}

		public static Action GetMeleeAttackAction(Pawn pawn, LocalTargetInfo target, out string failStr)
		{
			CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			failStr = "";
			if (comp == null || comp.moveTracker == null)
			{
				return null;
			}
			else if (target.IsValid && !pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly))
			{
				failStr = "NoPath".Translate();
			}
			else if (!PokemonAttackGizmoUtility.CanUseAnyMeleeVerb(pawn))
			{
				failStr = "Incapable".Translate();
			}
			else if (pawn == target.Thing)
			{
				failStr = "CannotAttackSelf".Translate();
			}
			else if (pawn.playerSettings.Master == null || pawn.playerSettings.Master.Map != pawn.Map)
			{
				failStr = "This Pokémon has no master";
			}
			else if (pawn.playerSettings.Master.Drafted == false)
			{
				failStr = "This Pokémon's master is not drafted";
			}
			else if (IntVec3Utility.DistanceTo(pawn.Position, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
			{
				failStr = "This Pokémon's master is too far";
			}
			else if (target != null && IntVec3Utility.DistanceTo(target.Cell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
			{
				failStr = "The target is too far from this Pokémon's master";
			}
			else
			{
				Pawn target2;
				if ((target2 = target.Thing as Pawn) == null || (!pawn.InSameExtraFaction(target2, ExtraFactionType.HomeFaction) && !pawn.InSameExtraFaction(target2, ExtraFactionType.MiniFaction)))
				{
					return delegate
					{
						Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_PokemonAttackMelee"), target);
						Pawn pawn2 = target.Thing as Pawn;
						if (pawn2 != null)
						{
							job.killIncappedTarget = pawn2.Downed;
						}
						pawn.jobs.TryTakeOrderedJob(job);
					};
				}
				failStr = "CannotAttackSameFactionMember".Translate();
			}
			failStr = failStr.CapitalizeFirst();
			return null;
		}
		public static FloatMenuOption DecoratePrioritizedTask(FloatMenuOption option, Pawn pawn, LocalTargetInfo target, string reservedText = "ReservedBy")
		{
			if (option.action == null)
			{
				return option;
			}
			if (pawn != null && !pawn.CanReserve(target) && pawn.CanReserve(target, 1, -1, null, ignoreOtherReservations: true))
			{
				Pawn pawn2 = pawn.Map.reservationManager.FirstRespectedReserver(target, pawn);
				if (pawn2 == null)
				{
					pawn2 = pawn.Map.physicalInteractionReservationManager.FirstReserverOf(target);
				}
				if (pawn2 != null)
				{
					option.Label = option.Label + ": " + reservedText.Translate(pawn2.LabelShort, pawn2);
				}
			}
			if (option.revalidateClickTarget != null && option.revalidateClickTarget != target.Thing)
			{
				Log.ErrorOnce($"Click target mismatch; {option.revalidateClickTarget} vs {target.Thing} in {option.Label}", 52753118);
			}
			option.revalidateClickTarget = target.Thing;
			return option;
		}
	}
}
