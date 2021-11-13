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
    class IncidentWorker_PokemonManhunterPack : IncidentWorker
	{
		private const float PointsFactor = 1f;

		private const int AnimalsStayDurationMin = 60000;

		private const int AnimalsStayDurationMax = 120000;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Map map = (Map)parms.target;
			IntVec3 result;
			if (Pokemon_ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(parms.points, map.Tile, out var _))
			{
				return RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Animal);
			}
			return false;
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			List<PawnKindDef> animalKind  = new List<PawnKindDef>();
			animalKind.Add(parms.pawnKind);
			if ((animalKind[0] == null && !Pokemon_ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(parms.points, map.Tile, out animalKind)) || Pokemon_ManhunterPackIncidentUtility.GetAnimalsCount(animalKind, parms.points) == 0)
			{
				return false;
			}
			IntVec3 result = parms.spawnCenter;
			if (!result.IsValid && !RCellFinder.TryFindRandomPawnEntryCell(out result, map, CellFinder.EdgeRoadChance_Animal))
			{
				return false;
			}
			if (animalKind[0].race.HasComp(typeof(CompPokemon)))
            {
				List<Pawn> list = Pokemon_ManhunterPackIncidentUtility.GeneratePokemonFamily_NewTmp(animalKind, map.Tile, parms.points * 1f, parms.pawnCount);
				Rot4 rot = Rot4.FromAngleFlat((map.Center - result).AngleFlat);
				for (int i = 0; i < list.Count; i++)
				{
					Pawn pawn = list[i];
					IntVec3 loc = CellFinder.RandomClosewalkCellNear(result, map, 10);
					QuestUtility.AddQuestTag(GenSpawn.Spawn(pawn, loc, map, rot), parms.questTag);
					pawn.health.AddHediff(HediffDefOf.Scaria);
					pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
					pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(60000, 120000);
				}
				SendStandardLetter("LetterLabelManhunterPackArrived".Translate(), "ManhunterPackArrived".Translate(animalKind[0].GetLabelPlural()), LetterDefOf.ThreatBig, parms, list[0]);
				Find.TickManager.slower.SignalForceNormalSpeedShort();
				LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
				LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Important);
				return true;
			}
			return false;
		}
	}
	public static class Pokemon_ManhunterPackIncidentUtility
	{
		public const int MinAnimalCount = 2;

		public const int MaxAnimalCount = 100;

		public const float MinPoints = 70f;

		public static float ManhunterAnimalWeight(PawnKindDef animal, float points)
		{
			points = Mathf.Max(points, 70f);
			if (animal.combatPower * 2f > points)
			{
				return 0f;
			}
			int num = Mathf.Min(Mathf.RoundToInt(points / animal.combatPower), 100);
			return Mathf.Clamp01(Mathf.InverseLerp(100f, 10f, num));
		}

		public static bool TryFindManhunterAnimalKind(float points, int tile, out List<PawnKindDef> allPokemonKind)
		{
			if (PokeWorldSettings.OkforPokemon())
			{
				IEnumerable<PawnKindDef> source = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef k) => k.RaceProps.Animal && k.race.HasComp(typeof(CompPokemon)) && PokeWorldSettings.GenerationAllowed(k.race.GetCompProperties<CompProperties_Pokemon>().generation) && k.canArriveManhunter && (tile == -1 || Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, k.race)));
				PawnKindDef singlePokemonKind = null;
				if (source.Any())
				{
					if (source.TryRandomElementByWeight((PawnKindDef a) => ManhunterAnimalWeight(a, points), out singlePokemonKind))
					{

					}
					else if (points > source.Min((PawnKindDef a) => a.combatPower) * 2f)
					{
						singlePokemonKind = source.MaxBy((PawnKindDef a) => a.combatPower);						
					}
                    else
                    {
						allPokemonKind = null;
						return false;
					}
					int evolutionLine = (singlePokemonKind.race.comps.Find((CompProperties y) => y.compClass == typeof(CompPokemon)) as CompProperties_Pokemon).evolutionLine;
					allPokemonKind = DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.RaceProps.Animal && x.canArriveManhunter && x.race.HasComp(typeof(CompPokemon)) && (x.race.comps.Find((CompProperties y) => y.compClass == typeof(CompPokemon)) as CompProperties_Pokemon).evolutionLine == evolutionLine && (tile == -1 || Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, x.race))).ToList();
					return true;
				}
			}
			allPokemonKind = null;
			return false;
		}

		public static int GetAnimalsCount(List<PawnKindDef> animalKind, float points)
		{
			return Mathf.Clamp(Mathf.RoundToInt(points / animalKind[0].combatPower), 2, 100);
		}
		
		public static List<Pawn> GeneratePokemonFamily_NewTmp(List<PawnKindDef> allPokemonKind, int tile, float points, int animalCount = 0)
		{
			List<Pawn> list = new List<Pawn>();
			int num = 0;
			float pointsLeft = points * 1f;
			PawnKindDef result2;
			for (; pointsLeft > 0f; pointsLeft -= result2.combatPower)
			{
				num++;
				if (num > 1000)
				{
					Log.Error("Too many iterations.");
					break;
				}
				if (!allPokemonKind.Where((PawnKindDef x) => x.combatPower <= pointsLeft).TryRandomElement(out result2))
				{
					break;
				}
				Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(result2, null, PawnGenerationContext.NonPlayer, tile));
				list.Add(pawn);
			}
			return list;
		}

		[DebugOutput]
		public static void ManhunterResults()
		{
			List<PawnKindDef> candidates = (from k in DefDatabase<PawnKindDef>.AllDefs
											where k.RaceProps.Animal && k.canArriveManhunter
											orderby 0f - k.combatPower
											select k).ToList();
			List<float> list = new List<float>();
			for (int i = 0; i < 30; i++)
			{
				list.Add(20f * Mathf.Pow(1.25f, i));
			}
			DebugTables.MakeTablesDialog(list, (float points) => points.ToString("F0") + " pts", candidates, (PawnKindDef candidate) => candidate.defName + " (" + candidate.combatPower.ToString("F0") + ")", delegate (float points, PawnKindDef candidate)
			{
				float num = candidates.Sum((PawnKindDef k) => ManhunterAnimalWeight(k, points));
				float num2 = ManhunterAnimalWeight(candidate, points);
				return (num2 == 0f) ? "0%" : string.Format("{0}%, {1}", (num2 * 100f / num).ToString("F0"), Mathf.Max(Mathf.RoundToInt(points / candidate.combatPower), 1));
			});
		}
	}
}
