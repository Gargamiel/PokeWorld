using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PokeWorld
{
    class IncidentWorker_Ambush_PokemonManhunterPack : IncidentWorker_Ambush
	{
		private const float ManhunterAmbushPointsFactor = 0.75f;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			List<PawnKindDef> animalKind;
			return Pokemon_ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(AdjustedPoints(parms.points), -1, out animalKind);
		}

		protected override List<Pawn> GeneratePawns(IncidentParms parms)
		{
			if (!Pokemon_ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(AdjustedPoints(parms.points), parms.target.Tile, out var animalKind) && !Pokemon_ManhunterPackIncidentUtility.TryFindManhunterAnimalKind(AdjustedPoints(parms.points), -1, out animalKind))
			{
				Log.Error(string.Concat("Could not find any valid animal kind for ", def, " incident."));
				return new List<Pawn>();
			}
			return Pokemon_ManhunterPackIncidentUtility.GeneratePokemonFamily_NewTmp(animalKind, parms.target.Tile, AdjustedPoints(parms.points));
		}

		protected override void PostProcessGeneratedPawnsAfterSpawning(List<Pawn> generatedPawns)
		{
			for (int i = 0; i < generatedPawns.Count; i++)
			{
				generatedPawns[i].health.AddHediff(HediffDefOf.Scaria);
				generatedPawns[i].mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
			}
		}

		private float AdjustedPoints(float basePoints)
		{
			return basePoints * 0.75f;
		}

		protected override string GetLetterText(Pawn anyPawn, IncidentParms parms)
		{
			Caravan caravan = parms.target as Caravan;
			return string.Format(def.letterText, (caravan != null) ? caravan.Name : "yourCaravan".TranslateSimple(), anyPawn.GetKindLabelPlural()).CapitalizeFirst();
		}
	}
}
