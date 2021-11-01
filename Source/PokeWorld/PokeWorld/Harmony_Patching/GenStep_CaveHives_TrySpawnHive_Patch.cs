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
	[HarmonyPatch(typeof(GenStep_CaveHives))]
	[HarmonyPatch("TrySpawnHive")]
	public class GenStep_CaveHives_TrySpawnHive_Patch
	{
		public static bool Prefix(GenStep_CaveHives __instance, Map __0)
		{
			
			if (PokeWorldSettings.OkforPokemon())
			{
				var field1 = __instance.GetType().GetField("possibleSpawnCells", BindingFlags.NonPublic | BindingFlags.Instance);
				List<IntVec3> possibleSpawnCells = (List<IntVec3>)field1.GetValue(__instance);

				var field2 = __instance.GetType().GetField("spawnedHives", BindingFlags.NonPublic | BindingFlags.Instance);
				List<Hive> spawnedHives = (List<Hive>)field2.GetValue(__instance);


				var method = __instance.GetType().GetMethod("TryFindHiveSpawnCell", BindingFlags.NonPublic | BindingFlags.Instance);
				object[] parameters = new object[] { __0, null };
				object result = method.Invoke(__instance, parameters);
				bool blResult = (bool)result;
				if (blResult)
				{
					IntVec3 spawnCell = (IntVec3)parameters[1];
					possibleSpawnCells.Remove(spawnCell);
					ThingDef hiveDef = PokemonInfestationUtility.GetRandomPokemonHiveDef();
					Hive hive = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(hiveDef), spawnCell, __0);
					hive.SetFaction(Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First());
					hive.PawnSpawner.aggressive = false;
					(from x in hive.GetComps<CompSpawner>()
					 where x.PropsSpawner.thingToSpawn == ThingDefOf.GlowPod
					 select x).First().TryDoSpawn();
					hive.PawnSpawner.SpawnPawnsUntilPoints(Rand.Range(200f, 500f));
					hive.PawnSpawner.canSpawnPawns = false;
					hive.GetComp<CompSpawnerHives>().canSpawnHives = false;
					spawnedHives.Add(hive);
				}
				return false;
			}
			return true;			
		}
	}
}
