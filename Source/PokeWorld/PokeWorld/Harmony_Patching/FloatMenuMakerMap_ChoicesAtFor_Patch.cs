using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using UnityEngine;
using HarmonyLib;
using System.Reflection;


namespace PokeWorld
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("ChoicesAtFor")]
    class FloatMenuMakerMap_ChoicesAtFor_Patch
	{
		private static FloatMenuOption[] equivalenceGroupTempStorage = null;

		public static bool Prefix(Vector3 __0, Pawn __1, ref List<FloatMenuOption> __result)
        {
			CompPokemon comp = __1.TryGetComp<CompPokemon>();
			if (comp != null)
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				if (__1.MentalStateDef == null && __1.playerSettings != null && __1.playerSettings.Master != null && __1.playerSettings.Master.Drafted)
				{
					IntVec3 intVec = IntVec3.FromVector3(__0);
					if (!intVec.InBounds(__1.Map))
					{
						return false;
					}
					if (__1.Map != Find.CurrentMap)
					{
						return false;
					}
					//makingFor = __1;
					try
					{
						if (intVec.Fogged(__1.Map))
						{
							FloatMenuOption floatMenuOption = GotoLocationOption(intVec, __1);
							if (floatMenuOption != null)
							{
								if (!floatMenuOption.Disabled)
								{
									list.Add(floatMenuOption);
									__result = list;
									return false;
								}
								__result = list;
								return false;
							}
							__result = list;
							return false;
						}
						AddDraftedOrders(__0, __1, list);
						foreach (FloatMenuOption item in __1.GetExtraFloatMenuOptionsFor(intVec))
						{
							list.Add(item);
						}
						__result = list;
						return false;
					}
					finally
					{
						//makingFor = null;
					}				
				}
				__result = list;
				return false;
			}
			return true;
		}
		private static FloatMenuOption GotoLocationOption(IntVec3 clickCell, Pawn pawn)
		{
			int num = GenRadial.NumCellsInRadius(2.9f);
			IntVec3 curLoc;
			for (int i = 0; i < num; i++)
			{
				curLoc = GenRadial.RadialPattern[i] + clickCell;
				if (!curLoc.Standable(pawn.Map))
				{
					continue;
				}
				if (curLoc != pawn.Position)
				{
					if (!PokemonMasterUtility.IsPokemonMasterDrafted(pawn))
					{
						return new FloatMenuOption("Cannot go here: Pokémon has no master", null);
					}
					if (!pawn.CanReach(curLoc, PathEndMode.OnCell, Danger.Deadly))
					{
						return new FloatMenuOption("CannotGoNoPath".Translate(), null);
					}
					if (IntVec3Utility.DistanceTo(clickCell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
					{
						return new FloatMenuOption("Cannot go here: too far from master", null);
					}				
					Action action = delegate
					{
						IntVec3 intVec = PokemonRCellFinder.BestOrderedGotoDestNear(curLoc, pawn);
						Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("PW_PokemonGotoForced"), intVec);
						job.playerForced = true;			
						if (pawn.Map.exitMapGrid.IsExitCell(UI.MouseCell()))
						{
							job.exitMapOnArrival = true;
						}
						else if (!pawn.Map.IsPlayerHome && !pawn.Map.exitMapGrid.MapUsesExitGrid && CellRect.WholeMap(pawn.Map).IsOnEdge(UI.MouseCell(), 3) && pawn.Map.Parent.GetComponent<FormCaravanComp>() != null && MessagesRepeatAvoider.MessageShowAllowed("MessagePlayerTriedToLeaveMapViaExitGrid-" + pawn.Map.uniqueID, 60f))
						{
							if (pawn.Map.Parent.GetComponent<FormCaravanComp>().CanFormOrReformCaravanNow)
							{
								Messages.Message("MessagePlayerTriedToLeaveMapViaExitGrid_CanReform".Translate(), pawn.Map.Parent, MessageTypeDefOf.RejectInput, historical: false);
							}
							else
							{
								Messages.Message("MessagePlayerTriedToLeaveMapViaExitGrid_CantReform".Translate(), pawn.Map.Parent, MessageTypeDefOf.RejectInput, historical: false);
							}
						}
						if (pawn.jobs.TryTakeOrderedJob(job))
						{
							FleckMaker.Static(intVec, pawn.Map, FleckDefOf.FeedbackGoto);
						}
					};
					return new FloatMenuOption("GoHere".Translate(), action, MenuOptionPriority.GoHere)
					{
						autoTakeable = true,
						autoTakeablePriority = 10f
					};
				}
				return null;
			}
			return null;
		}
		private static void AddDraftedOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			IntVec3 clickCell = IntVec3.FromVector3(clickPos);
			foreach (LocalTargetInfo item in GenUI.TargetsAt(clickPos, TargetingParameters.ForAttackHostile(), thingsOnly: true))
			{
				LocalTargetInfo attackTarg = item;
				CompPokemon comp = pawn.TryGetComp<CompPokemon>();
				if (comp != null && comp.moveTracker != null && PokemonAttackGizmoUtility.CanUseAnyRangedVerb(pawn))
				{
					string failStr;
					Action rangedAct = PokemonFloatMenuUtility.GetRangedAttackAction(pawn, attackTarg, out failStr);
					string text = "FireAt".Translate(attackTarg.Thing.Label, attackTarg.Thing);
					FloatMenuOption floatMenuOption = new FloatMenuOption("", null, MenuOptionPriority.High, null, item.Thing);
					if (rangedAct == null)
					{
						text = text + ": " + failStr;
					}
					else if (PokemonMasterUtility.IsPokemonMasterDrafted(pawn))
					{
						text = "Cannot go here: Pokémon has no master";
					}
					else if (IntVec3Utility.DistanceTo(clickCell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
					{
						text = "Cannot go here: too far from master";
					}
					else
					{
						floatMenuOption.autoTakeable = !attackTarg.HasThing || attackTarg.Thing.HostileTo(Faction.OfPlayer);
						floatMenuOption.autoTakeablePriority = 40f;
						floatMenuOption.action = delegate
						{
							FleckMaker.Static(attackTarg.Thing.DrawPos, attackTarg.Thing.Map, FleckDefOf.FeedbackShoot);
							rangedAct();
						};
					}
					floatMenuOption.Label = text;
					opts.Add(floatMenuOption);
				}
				string failStr2;
				Action meleeAct = PokemonFloatMenuUtility.GetMeleeAttackAction(pawn, attackTarg, out failStr2);
				Pawn pawn2 = attackTarg.Thing as Pawn;
				string text2 = ((pawn2 == null || !pawn2.Downed) ? ((string)"MeleeAttack".Translate(attackTarg.Thing.Label, attackTarg.Thing)) : ((string)"MeleeAttackToDeath".Translate(attackTarg.Thing.Label, attackTarg.Thing)));
				MenuOptionPriority priority = ((!attackTarg.HasThing || !pawn.HostileTo(attackTarg.Thing)) ? MenuOptionPriority.VeryLow : MenuOptionPriority.AttackEnemy);
				FloatMenuOption floatMenuOption2 = new FloatMenuOption("", null, priority, null, attackTarg.Thing);
				if (meleeAct == null)
				{
					text2 = text2 + ": " + failStr2.CapitalizeFirst();
				}
				else if (pawn.playerSettings == null || pawn.playerSettings.Master == null || !pawn.playerSettings.Master.Spawned || pawn.playerSettings.Master.Map != pawn.Map)
				{
					text2 = "Cannot attack: Pokémon has no master";
				}
				else if (IntVec3Utility.DistanceTo(clickCell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
				{
					text2 = "Cannot attack: too far from master";
				}
				else
				{
					floatMenuOption2.autoTakeable = !attackTarg.HasThing || attackTarg.Thing.HostileTo(Faction.OfPlayer);
					floatMenuOption2.autoTakeablePriority = 30f;
					floatMenuOption2.action = delegate
					{
						FleckMaker.Static(attackTarg.Thing.DrawPos, attackTarg.Thing.Map, FleckDefOf.FeedbackMelee);
						meleeAct();
					};
				}
				floatMenuOption2.Label = text2;
				opts.Add(floatMenuOption2);
			}
			//AddJobGiverWorkOrders_NewTmp(clickPos, pawn, opts, drafted: true);
			FloatMenuOption floatMenuOption3 = GotoLocationOption(clickCell, pawn);
			if (floatMenuOption3 != null)
			{
				opts.Add(floatMenuOption3);
			}
		}
		private static void AddJobGiverWorkOrders_NewTmp(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts, bool drafted)
		{
			if (pawn.thinker.TryGetMainTreeThinkNode<JobGiver_Work>() == null)
			{
				return;
			}
			IntVec3 clickCell = IntVec3.FromVector3(clickPos);
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = true;
			targetingParameters.canTargetItems = true;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			foreach (Thing item in GenUI.ThingsUnderMouse(clickPos, 1f, targetingParameters))
			{
				bool flag = false;
				foreach (WorkTypeDef item2 in DefDatabase<WorkTypeDef>.AllDefsListForReading)
				{
					for (int i = 0; i < item2.workGiversByPriority.Count; i++)
					{
						WorkGiverDef workGiver2 = item2.workGiversByPriority[i];
						if (drafted && !workGiver2.canBeDoneWhileDrafted)
						{
							continue;
						}
						WorkGiver_Scanner workGiver_Scanner = workGiver2.Worker as WorkGiver_Scanner;
						if (workGiver_Scanner == null || !workGiver_Scanner.def.directOrderable)
						{
							continue;
						}
						JobFailReason.Clear();
						if ((!workGiver_Scanner.PotentialWorkThingRequest.Accepts(item) && (workGiver_Scanner.PotentialWorkThingsGlobal(pawn) == null || !workGiver_Scanner.PotentialWorkThingsGlobal(pawn).Contains(item))) || workGiver_Scanner.ShouldSkip(pawn, forced: true))
						{
							continue;
						}
						string text = null;
						Action action = null;
						PawnCapacityDef pawnCapacityDef = workGiver_Scanner.MissingRequiredCapacity(pawn);
						if (pawnCapacityDef != null)
						{
							text = "CannotMissingHealthActivities".Translate(pawnCapacityDef.label);
						}
						else
						{
							Job job = (workGiver_Scanner.HasJobOnThing(pawn, item, forced: true) ? workGiver_Scanner.JobOnThing(pawn, item, forced: true) : null);
							if (job == null)
							{
								if (JobFailReason.HaveReason)
								{
									text = (JobFailReason.CustomJobString.NullOrEmpty() ? ((string)"CannotGenericWork".Translate(workGiver_Scanner.def.verb, item.LabelShort, item)) : ((string)"CannotGenericWorkCustom".Translate(JobFailReason.CustomJobString)));
									text = text + ": " + JobFailReason.Reason.CapitalizeFirst();
								}
								else
								{
									if (!item.IsForbidden(pawn))
									{
										continue;
									}
									text = (item.Position.InAllowedArea(pawn) ? ((string)"CannotPrioritizeForbidden".Translate(item.Label, item)) : ((string)("CannotPrioritizeForbiddenOutsideAllowedArea".Translate() + ": " + pawn.playerSettings.EffectiveAreaRestriction.Label)));
								}
							}
							else
							{
								WorkTypeDef workType = workGiver_Scanner.def.workType;
								if (pawn.WorkTagIsDisabled(workGiver_Scanner.def.workTags))
								{
									text = "CannotPrioritizeWorkGiverDisabled".Translate(workGiver_Scanner.def.label);
								}
								else if (pawn.jobs.curJob != null && pawn.jobs.curJob.JobIsSameAs(job))
								{
									text = "CannotGenericAlreadyAm".Translate(workGiver_Scanner.PostProcessedGerund(job), item.LabelShort, item);
								}
								else if (pawn.workSettings.GetPriority(workType) == 0)
								{
									text = (pawn.WorkTypeIsDisabled(workType) ? ((string)"CannotPrioritizeWorkTypeDisabled".Translate(workType.gerundLabel)) : ((!"CannotPrioritizeNotAssignedToWorkType".CanTranslate()) ? ((string)"CannotPrioritizeWorkTypeDisabled".Translate(workType.pawnLabel)) : ((string)"CannotPrioritizeNotAssignedToWorkType".Translate(workType.gerundLabel))));
								}
								else if (job.def == JobDefOf.Research && item is Building_ResearchBench)
								{
									text = "CannotPrioritizeResearch".Translate();
								}
								else if (item.IsForbidden(pawn))
								{
									text = (item.Position.InAllowedArea(pawn) ? ((string)"CannotPrioritizeForbidden".Translate(item.Label, item)) : ((string)("CannotPrioritizeForbiddenOutsideAllowedArea".Translate() + ": " + pawn.playerSettings.EffectiveAreaRestriction.Label)));
								}
								else if (!pawn.CanReach(item, workGiver_Scanner.PathEndMode, Danger.Deadly))
								{
									text = (item.Label + ": " + "NoPath".Translate().CapitalizeFirst()).CapitalizeFirst();
								}
								else
								{
									text = "PrioritizeGeneric".Translate(workGiver_Scanner.PostProcessedGerund(job), item.Label);
									Job localJob2 = job;
									WorkGiver_Scanner localScanner2 = workGiver_Scanner;
									job.workGiverDef = workGiver_Scanner.def;
									action = delegate
									{
										if (pawn.jobs.TryTakeOrderedJobPrioritizedWork(localJob2, localScanner2, clickCell) && workGiver2.forceMote != null)
										{
											MoteMaker.MakeStaticMote(clickCell, pawn.Map, workGiver2.forceMote);
										}
									};
								}
							}
						}
						if (DebugViewSettings.showFloatMenuWorkGivers)
						{
							text += $" (from {workGiver2.defName})";
						}
						FloatMenuOption menuOption = PokemonFloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, action), pawn, item);
						if (drafted && workGiver2.autoTakeablePriorityDrafted != -1)
						{
							menuOption.autoTakeable = true;
							menuOption.autoTakeablePriority = workGiver2.autoTakeablePriorityDrafted;
						}
						if (opts.Any((FloatMenuOption op) => op.Label == menuOption.Label))
						{
							continue;
						}
						if (workGiver2.equivalenceGroup != null)
						{
							if (equivalenceGroupTempStorage[workGiver2.equivalenceGroup.index] == null || (equivalenceGroupTempStorage[workGiver2.equivalenceGroup.index].Disabled && !menuOption.Disabled))
							{
								equivalenceGroupTempStorage[workGiver2.equivalenceGroup.index] = menuOption;
								flag = true;
							}
						}
						else
						{
							opts.Add(menuOption);
						}
					}
				}
				if (!flag)
				{
					continue;
				}
				for (int j = 0; j < equivalenceGroupTempStorage.Length; j++)
				{
					if (equivalenceGroupTempStorage[j] != null)
					{
						opts.Add(equivalenceGroupTempStorage[j]);
						equivalenceGroupTempStorage[j] = null;
					}
				}
			}
			foreach (WorkTypeDef item3 in DefDatabase<WorkTypeDef>.AllDefsListForReading)
			{
				for (int k = 0; k < item3.workGiversByPriority.Count; k++)
				{
					WorkGiverDef workGiver = item3.workGiversByPriority[k];
					if (drafted && !workGiver.canBeDoneWhileDrafted)
					{
						continue;
					}
					WorkGiver_Scanner workGiver_Scanner2 = workGiver.Worker as WorkGiver_Scanner;
					if (workGiver_Scanner2 == null || !workGiver_Scanner2.def.directOrderable)
					{
						continue;
					}
					JobFailReason.Clear();
					if (!workGiver_Scanner2.PotentialWorkCellsGlobal(pawn).Contains(clickCell) || workGiver_Scanner2.ShouldSkip(pawn, forced: true))
					{
						continue;
					}
					Action action2 = null;
					string label = null;
					PawnCapacityDef pawnCapacityDef2 = workGiver_Scanner2.MissingRequiredCapacity(pawn);
					if (pawnCapacityDef2 != null)
					{
						label = "CannotMissingHealthActivities".Translate(pawnCapacityDef2.label);
					}
					else
					{
						Job job2 = (workGiver_Scanner2.HasJobOnCell(pawn, clickCell, forced: true) ? workGiver_Scanner2.JobOnCell(pawn, clickCell, forced: true) : null);
						if (job2 == null)
						{
							if (JobFailReason.HaveReason)
							{
								if (!JobFailReason.CustomJobString.NullOrEmpty())
								{
									label = "CannotGenericWorkCustom".Translate(JobFailReason.CustomJobString);
								}
								else
								{
									label = "CannotGenericWork".Translate(workGiver_Scanner2.def.verb, "AreaLower".Translate());
								}
								label = label + ": " + JobFailReason.Reason.CapitalizeFirst();
							}
							else
							{
								if (!clickCell.IsForbidden(pawn))
								{
									continue;
								}
								if (!clickCell.InAllowedArea(pawn))
								{
									label = "CannotPrioritizeForbiddenOutsideAllowedArea".Translate() + ": " + pawn.playerSettings.EffectiveAreaRestriction.Label;
								}
								else
								{
									label = "CannotPrioritizeCellForbidden".Translate();
								}
							}
						}
						else
						{
							WorkTypeDef workType2 = workGiver_Scanner2.def.workType;
							if (pawn.jobs.curJob != null && pawn.jobs.curJob.JobIsSameAs(job2))
							{
								label = "CannotGenericAlreadyAmCustom".Translate(workGiver_Scanner2.PostProcessedGerund(job2));
							}
							else if (pawn.workSettings.GetPriority(workType2) == 0)
							{
								if (pawn.WorkTypeIsDisabled(workType2))
								{
									label = "CannotPrioritizeWorkTypeDisabled".Translate(workType2.gerundLabel);
								}
								else if ("CannotPrioritizeNotAssignedToWorkType".CanTranslate())
								{
									label = "CannotPrioritizeNotAssignedToWorkType".Translate(workType2.gerundLabel);
								}
								else
								{
									label = "CannotPrioritizeWorkTypeDisabled".Translate(workType2.pawnLabel);
								}
							}
							else if (clickCell.IsForbidden(pawn))
							{
								if (!clickCell.InAllowedArea(pawn))
								{
									label = "CannotPrioritizeForbiddenOutsideAllowedArea".Translate() + ": " + pawn.playerSettings.EffectiveAreaRestriction.Label;
								}
								else
								{
									label = "CannotPrioritizeCellForbidden".Translate();
								}
							}
							else if (!pawn.CanReach(clickCell, PathEndMode.Touch, Danger.Deadly))
							{
								label = "AreaLower".Translate().CapitalizeFirst() + ": " + "NoPath".Translate().CapitalizeFirst();
							}
							else
							{
								label = "PrioritizeGeneric".Translate(workGiver_Scanner2.PostProcessedGerund(job2), "AreaLower".Translate());
								Job localJob = job2;
								WorkGiver_Scanner localScanner = workGiver_Scanner2;
								job2.workGiverDef = workGiver_Scanner2.def;
								action2 = delegate
								{
									if (pawn.jobs.TryTakeOrderedJobPrioritizedWork(localJob, localScanner, clickCell) && workGiver.forceMote != null)
									{
										MoteMaker.MakeStaticMote(clickCell, pawn.Map, workGiver.forceMote);
									}
								};
							}
						}
					}
					if (!opts.Any((FloatMenuOption op) => op.Label == label.TrimEnd()))
					{
						FloatMenuOption floatMenuOption = PokemonFloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action2), pawn, clickCell);
						if (drafted && workGiver.autoTakeablePriorityDrafted != -1)
						{
							floatMenuOption.autoTakeable = true;
							floatMenuOption.autoTakeablePriority = workGiver.autoTakeablePriorityDrafted;
						}
						opts.Add(floatMenuOption);
					}
				}
			}
		}

	}
}
