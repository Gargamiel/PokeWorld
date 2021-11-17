using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld.Planet;
using Verse;

namespace PokeWorld
{
	public class LegendaryPokemonComp : ImportantPawnComp, IThingHolder
	{
		protected override string PawnSaveKey => "legendary";

		protected override void RemovePawnOnWorldObjectRemoved()
		{
			if (!pawn.Any)
			{
				return;
			}
			if (!pawn[0].Dead)
			{
				HealthUtility.HealNonPermanentInjuriesAndRestoreLegs(pawn[0]);
			}
			pawn.ClearAndDestroyContentsOrPassToWorld();
		}

		public override string CompInspectStringExtra()
		{
			if (pawn != null && pawn.Any)
			{
				return "PW_LegendaryTitle".Translate(pawn[0].LabelCap);
			}
			return null;
		}
	}
}
