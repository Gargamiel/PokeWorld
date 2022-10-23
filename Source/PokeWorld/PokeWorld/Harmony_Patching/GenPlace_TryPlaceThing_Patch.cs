using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using HarmonyLib;
using RimWorld;

namespace PokeWorld
{
	[HarmonyPatch(typeof(GenPlace), "TryPlaceThing", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(ThingPlaceMode), typeof(Action<Thing, int>), typeof(Predicate<IntVec3>), typeof(Rot4) })]
	class GenPlace_TryPlaceThing_Patch
	{
		public static bool Prefix(Thing __0, IntVec3 __1, Map __2, ref bool __result)
		{			
			if(__0.def == DefDatabase<ThingDef>.GetNamed("PW_Porygon"))
            {
				__result = true;
				Pawn revivedPokemon = PokemonGeneratorUtility.GenerateAndSpawnNewPokemon(DefDatabase<PawnKindDef>.GetNamed("PW_Porygon"), Faction.OfPlayer, __1, __2, null, true, false);
				Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(DefDatabase<PawnKindDef>.GetNamed("PW_Porygon"));
				//Messages.Message("PW_CraftedPokemon".Translate(actor.LabelShortCap, revivedPokemon.KindLabel), revivedPokemon, MessageTypeDefOf.PositiveEvent);
				return false;
            }
			return true;
		}
	}
}