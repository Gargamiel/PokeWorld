using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using RimWorld.Planet;

namespace PokeWorld
{
    class DamageWorker_TryCatch : DamageWorker
    {
		public float bonusBall;
		public override DamageResult Apply(DamageInfo dinfo, Thing thing)
		{
			if (thing.GetType() == typeof(Pawn))
			{
				Pawn pawn = thing as Pawn;
				Pawn instigator = dinfo.Instigator as Pawn;
				Pawn targetPawn = null;
				if (pawn == null)
				{
					return base.Apply(dinfo, thing);
				}
				if (dinfo.IntendedTarget != null && dinfo.IntendedTarget.GetType() == typeof(Pawn))
				{
					targetPawn = dinfo.IntendedTarget as Pawn;
				}				
				if (pawn == targetPawn)
				{
					CompPokemon compPokemon = pawn.TryGetComp<CompPokemon>();													
					if (pawn.AnimalOrWildMan() && (pawn.Faction == null || pawn.Faction == Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First()) && compPokemon != null && !pawn.Downed && compPokemon.tryCatchCooldown <= 0)
					{
						CompProperties_Pokeball compPokeballBelt = dinfo.Weapon.GetCompProperties<CompProperties_Pokeball>();
						float catchRate = pawn.GetStatValue(DefDatabase<StatDef>.GetNamed("PW_CatchRate"));
						if (catchRate > 0)
						{
							float currentHealthPercent = pawn.health.summaryHealth.SummaryHealthPercent;
							float aValue = (1 - ((2 / 3f) * currentHealthPercent)) * catchRate * bonusBall;
							float bValue = aValue / 255;
							float rand = Rand.Range(0f, 1f);
							if (bValue > rand)
							{
								CompXpEvGiver compXpEvGiver = pawn.TryGetComp<CompXpEvGiver>();
								if(compXpEvGiver != null)
                                {
									compXpEvGiver.DistributeAfterCatch();
								}
								InteractionWorker_RecruitAttempt.DoRecruit(instigator, pawn);
								pawn.training.Train(DefDatabase<TrainableDef>.GetNamed("Obedience"), instigator, true);
								pawn.training.SetWantedRecursive(DefDatabase<TrainableDef>.GetNamed("Obedience"), true);
								pawn.ClearMind();
								compPokemon.ballDef = compPokeballBelt.ballDef;
								Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(pawn.kindDef);
								PutInBallUtility.PutPokemonInBall(pawn);
								if(instigator.Faction != null && instigator.Faction == Faction.OfPlayer && instigator.skills != null && !instigator.skills.GetSkill(SkillDefOf.Animals).TotallyDisabled)
                                {
									instigator.skills.Learn(SkillDefOf.Animals, compPokemon.levelTracker.level * 50);
								}
							}
							else
							{
								if (instigator.Faction != null && instigator.Faction == Faction.OfPlayer && instigator.skills != null && !instigator.skills.GetSkill(SkillDefOf.Animals).TotallyDisabled)
								{
									instigator.skills.Learn(SkillDefOf.Animals, compPokemon.levelTracker.level * 10);
								}
								compPokemon.tryCatchCooldown = 120;
								string text = "PW_TextMoteCatchFailed".Translate(bValue.ToStringPercent());
								MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 8f);
                                if (Rand.Chance(pawn.RaceProps.manhunterOnTameFailChance))
                                {
									if (pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter))
									{
										string text2 = "AnimalManhunterFromTaming".Translate(pawn.Label, pawn.Named("PAWN")).AdjustedFor(pawn);
										GlobalTargetInfo target = pawn;
										int num = 1;
										if (Find.Storyteller.difficulty.allowBigThreats && Rand.Value < 0.5f)
										{
											Room pawnRoom = pawn.GetRoom();
											List<Pawn> raceMates = pawn.Map.mapPawns.AllPawnsSpawned;
											for (int i = 0; i < raceMates.Count; i++)
											{
												if (pawn != raceMates[i] && raceMates[i].def == pawn.def && raceMates[i].Faction == pawn.Faction && raceMates[i].Position.InHorDistOf(pawn.Position, 24f) && raceMates[i].GetRoom() == pawnRoom)
												{
													if (raceMates[i].mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter))
													{
														num++;
													}												
												}
											}
											if (num > 1)
											{
												target = new TargetInfo(pawn.Position, pawn.Map);
												text2 += "\n\n";
												text2 += "AnimalManhunterOthers".Translate(pawn.kindDef.GetLabelPlural(), pawn);
											}
										}
										string value = (pawn.RaceProps.Animal ? pawn.Label : pawn.def.label);
										string str = "LetterLabelAnimalManhunterRevenge".Translate(value).CapitalizeFirst();
										Find.LetterStack.ReceiveLetter(str, text2, (num == 1) ? LetterDefOf.ThreatSmall : LetterDefOf.ThreatBig, target);
									}									
								}
                                else if(!pawn.mindState.mentalStateHandler.InMentalState && ((Find.TickManager.TicksGame - pawn.LastAttackTargetTick) > 600))
                                {
									pawn.mindState.StartFleeingBecauseOfPawnAction(dinfo.Instigator);
								}
							}
						}
					}		
					else if (dinfo.Weapon == DefDatabase<ThingDef>.GetNamed("PW_MasterBallBelt") && pawn.def == ThingDefOf.Human)
                    {
						float rand = Rand.Range(0f, 1f);
						if (rand < 0.1f)
						{
							CompProperties_Pokeball compPokeballBelt = dinfo.Weapon.GetCompProperties<CompProperties_Pokeball>();
							HealthUtility.DamageUntilDead(pawn);
							PutInBallUtility.PutCorpseInBall(pawn.Corpse, compPokeballBelt.ballDef);							
						}
                        else
                        {
							string text = "PW_TextMoteCatchFailed".Translate(0.1f.ToStringPercent());
							MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 8f);
						}
					}
					else if (compPokemon == null)
					{
						string text = "PW_TextMoteCatchFailedNotPokemon".Translate();
						MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 8f);
					}
					else if (pawn.Faction != null && pawn.Faction != Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First())
                    {
						string text = "PW_TextMoteCatchFailedAlreadyOwned".Translate();
						MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 8f);
					}
					else if (pawn.Downed)
					{
						string text = "PW_TextMoteCatchFailedFainted".Translate();
						MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 8f);
					}
					else if (compPokemon.tryCatchCooldown > 0)
					{
						string text = "PW_TextMoteCatchFailedDodged".Translate();
						MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 8f);
					}					
				}			
			}
			return base.Apply(dinfo, thing);
		}
        public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
        {
			bonusBall = ((PokeBallExplosion)explosion).bonusBall;
            base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
        }
    }
}
