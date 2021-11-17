using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;
using UnityEngine;



namespace PokeWorld
{
    public class IncidentWorker_PokemonInfestation : IncidentWorker
	{
		public const float HivePoints = 220f;

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IntVec3 cell;
			if (base.CanFireNowSub(parms) && HiveUtility.TotalSpawnedHivesCount(map) < 30)
			{
				return InfestationCellFinder.TryFindCell(out cell, map);
			}
			return false;
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			if (PokeWorldSettings.OkforPokemon() && PokeWorldSettings.allowPokemonInfestation)
			{
				Map map = (Map)parms.target;
				ThingDef hiveDef = PokemonInfestationUtility.GetInfestationPokemonHiveDef();
				TunnelPokemonHiveSpawner t = PokemonInfestationUtility.SpawnTunnels(hiveDef, Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1), map);
				SendStandardLetter(parms, t);
				Find.TickManager.slower.SignalForceNormalSpeedShort();
				return true;
			}
			else
			{
				return false;
			}			
		}
		
	}

	public static class PokemonInfestationUtility
	{
		public static TunnelPokemonHiveSpawner SpawnTunnels(ThingDef hiveDef, int hiveCount, Map map, bool spawnAnywhereIfNoGoodCell = false, bool ignoreRoofedRequirement = false, string questTag = null)
		{
			if (!InfestationCellFinder.TryFindCell(out var cell, map))
			{
				if (!spawnAnywhereIfNoGoodCell)
				{
					return null;
				}
				if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(delegate (IntVec3 x)
				{
					if (!x.Standable(map) || x.Fogged(map))
					{
						return false;
					}
					bool flag = false;
					int num = GenRadial.NumCellsInRadius(3f);
					for (int j = 0; j < num; j++)
					{
						IntVec3 c = x + GenRadial.RadialPattern[j];
						if (c.InBounds(map))
						{
							RoofDef roof = c.GetRoof(map);
							if (roof != null && roof.isThickRoof)
							{
								flag = true;
								break;
							}
						}
					}
					return flag ? true : false;
				}, map, out cell))
				{
					return null;
				}
			}
			TunnelPokemonHiveSpawner thing = (TunnelPokemonHiveSpawner)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("PW_TunnelPokemonHiveSpawner"));
			thing.pokemonHiveDef = hiveDef;
			GenSpawn.Spawn(thing, cell, map, WipeMode.FullRefund);
			QuestUtility.AddQuestTag(thing, questTag);
			for (int i = 0; i < hiveCount - 1; i++)
			{
				cell = CompSpawnerHives.FindChildHiveLocation(thing.Position, map, hiveDef, hiveDef.GetCompProperties<CompProperties_SpawnerHives>(), ignoreRoofedRequirement, allowUnreachable: true);
				if (cell.IsValid)
				{
					thing = (TunnelPokemonHiveSpawner)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("PW_TunnelPokemonHiveSpawner"));
					thing.pokemonHiveDef = hiveDef;
					GenSpawn.Spawn(thing, cell, map, WipeMode.FullRefund); 
					QuestUtility.AddQuestTag(thing, questTag);
				}
			}
			return thing;
		}
		public static ThingDef GetNaturalPokemonHiveDef()
		{
			ThingDef def = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.defName == "PW_PokemonHiveGeodude"
																|| x.defName == "PW_PokemonHiveOnix"
																|| x.defName == "PW_PokemonHiveParas"
																|| x.defName == "PW_PokemonHiveDiglett"
																|| x.defName == "PW_PokemonHiveRhyhorn"
																|| x.defName == "PW_PokemonHivePhanpy"
																|| x.defName == "PW_PokemonHiveAron").RandomElement();
			return def;
		}
		public static ThingDef GetInfestationPokemonHiveDef()
		{
			ThingDef def = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.defName == "PW_InsectPokemonHive"
																|| x.defName == "PW_GroundPokemonHive").RandomElement();
			return def;
		}
	}

	[StaticConstructorOnStartup]
	public class TunnelPokemonHiveSpawner : ThingWithComps
	{
		private int secondarySpawnTick;

		public bool spawnHive = true;

		public float insectsPoints;

		public bool spawnedByInfestationThingComp;

		public ThingDef pokemonHiveDef;

		private Sustainer sustainer;

		private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

		private readonly FloatRange ResultSpawnDelay = new FloatRange(26f, 30f);

		[TweakValue("Gameplay", 0f, 1f)]
		private static float DustMoteSpawnMTB = 0.2f;

		[TweakValue("Gameplay", 0f, 1f)]
		private static float FilthSpawnMTB = 0.3f;

		[TweakValue("Gameplay", 0f, 10f)]
		private static float FilthSpawnRadius = 3f;

		private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);

		private static List<ThingDef> filthTypes = new List<ThingDef>();

		public static void ResetStaticData()
		{
			filthTypes.Clear();
			filthTypes.Add(ThingDefOf.Filth_Dirt);
			filthTypes.Add(ThingDefOf.Filth_Dirt);
			filthTypes.Add(ThingDefOf.Filth_Dirt);
			filthTypes.Add(ThingDefOf.Filth_RubbleRock);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref secondarySpawnTick, "PW_secondarySpawnTick", 0);
			Scribe_Values.Look(ref spawnHive, "PW_spawnHive", defaultValue: true);
			Scribe_Values.Look(ref insectsPoints, "PW_insectsPoints", 0f);
			Scribe_Values.Look(ref spawnedByInfestationThingComp, "PW_spawnedByInfestationThingComp", defaultValue: false);
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			ResetStaticData();
			if (!respawningAfterLoad)
			{
				secondarySpawnTick = Find.TickManager.TicksGame + ResultSpawnDelay.RandomInRange.SecondsToTicks();
			}
			CreateSustainer();
		}

		public override void Tick()
		{
			if (!base.Spawned)
			{
				return;
			}
			sustainer.Maintain();
			Vector3 vector = base.Position.ToVector3Shifted();
			if (Rand.MTBEventOccurs(FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors), null, null, out var result))
			{
				FilthMaker.TryMakeFilth(result, base.Map, filthTypes.RandomElement());
			}
			if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
			{
				Vector3 loc = new Vector3(vector.x, 0f, vector.z);
				loc.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				FleckMaker.ThrowDustPuffThick(loc, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
			}
			if (secondarySpawnTick > Find.TickManager.TicksGame)
			{
				return;
			}
			sustainer.End();
			Map map = base.Map;
			IntVec3 position = base.Position;
			Destroy();
			if (spawnHive)
			{
				Hive obj = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(pokemonHiveDef), position, map);
				obj.SetFaction(Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First());
				obj.questTags = questTags;
				foreach (CompSpawner comp in obj.GetComps<CompSpawner>())
				{
					if (comp.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly)
					{
						comp.TryDoSpawn();
						break;
					}
				}
			}
			if (!(insectsPoints > 0f))
			{
				return;
			}
			insectsPoints = Mathf.Max(insectsPoints, pokemonHiveDef.GetCompProperties<CompProperties_SpawnerPawn>().spawnablePawnKinds.Min((PawnKindDef x) => x.combatPower));
			float pointsLeft = insectsPoints;
			List<Pawn> list = new List<Pawn>();
			int num = 0;
			PawnKindDef result2;
			for (; pointsLeft > 0f; pointsLeft -= result2.combatPower)
			{
				num++;
				if (num > 1000)
				{
					Log.Error("Too many iterations.");
					break;
				}
				if (!pokemonHiveDef.GetCompProperties<CompProperties_SpawnerPawn>().spawnablePawnKinds.Where((PawnKindDef x) => x.combatPower <= pointsLeft).TryRandomElement(out result2))
				{
					break;
				}
				Pawn pawn = PawnGenerator.GeneratePawn(result2, Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First());
				GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(position, map, 2), map);
				pawn.mindState.spawnedByInfestationThingComp = spawnedByInfestationThingComp;
				list.Add(pawn);
			}
			if (list.Any())
			{
				LordMaker.MakeNewLord(Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First(), new LordJob_AssaultColony(Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First(), canKidnap: true, canTimeoutOrFlee: false), map, list);
			}
		}

		public override void Draw()
		{
			Rand.PushState();
			Rand.Seed = thingIDNumber;
			for (int i = 0; i < 6; i++)
			{
				DrawDustPart(Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f) * (float)Rand.Sign * 4f, Rand.Range(1f, 1.5f));
			}
			Rand.PopState();
		}

		private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
		{
			float num = (Find.TickManager.TicksGame - secondarySpawnTick).TicksToSeconds();
			Vector3 pos = base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Filth);
			pos.y += 3f / 70f * Rand.Range(0f, 1f);
			Color value = new Color(0.470588237f, 98f / 255f, 83f / 255f, 0.7f);
			matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
			Graphics.DrawMesh(MeshPool.plane10, matrix, TunnelMaterial, 0, null, 0, matPropertyBlock);
		}

		private void CreateSustainer()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				SoundDef tunnel = SoundDefOf.Tunnel;
				sustainer = tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
			});
		}
	}
}
