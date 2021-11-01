using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace PokeWorld
{
    [HarmonyPatch(typeof(WildAnimalSpawner))]
    [HarmonyPatch("SpawnRandomWildAnimalAt")]
    class WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch
    {
        public static bool Prefix(WildAnimalSpawner __instance, IntVec3 __0, ref bool __result)
        {
			var foo = __instance.GetType().GetField("map", BindingFlags.NonPublic | BindingFlags.Instance);
			Map map = (Map)foo.GetValue(__instance);

			PawnKindDef pawnKindDef = null;
			if (PokeWorldSettings.OkforPokemon())
			{
				pawnKindDef = map.Biome.AllWildAnimals.Where((PawnKindDef a) => map.mapTemperature.SeasonAcceptableFor(a.race) && a.race.HasComp(typeof(CompPokemon))).RandomElementByWeight((PawnKindDef def) => map.Biome.CommonalityOfAnimal(def) / def.wildGroupSize.Average);
			}
			else
            {
				pawnKindDef = map.Biome.AllWildAnimals.Where((PawnKindDef a) => map.mapTemperature.SeasonAcceptableFor(a.race) && !a.race.HasComp(typeof(CompPokemon))).RandomElementByWeight((PawnKindDef def) => map.Biome.CommonalityOfAnimal(def) / def.wildGroupSize.Average);
			}				
			if (pawnKindDef == null)
			{
				Log.Error("No spawnable animals right now.");
				__result = false;
			}
			int randomInRange = pawnKindDef.wildGroupSize.RandomInRange;
			int radius = Mathf.CeilToInt(Mathf.Sqrt(pawnKindDef.wildGroupSize.max));
			for (int i = 0; i < randomInRange; i++)
			{
				IntVec3 loc2 = CellFinder.RandomClosewalkCellNear(__0, map, radius);
				GenSpawn.Spawn(PawnGenerator.GeneratePawn(pawnKindDef), loc2, map);
			}
			__result = true;
			return false;
        }
    }
}
