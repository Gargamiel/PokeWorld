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
						return new FloatMenuOption("PW_CannotGoNoMaster".Translate(), null);
					}
					if (!pawn.CanReach(curLoc, PathEndMode.OnCell, Danger.Deadly))
					{
						return new FloatMenuOption("CannotGoNoPath".Translate(), null);
					}
					if (IntVec3Utility.DistanceTo(clickCell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
					{
						return new FloatMenuOption("PW_CannotGoTooFarFromMaster".Translate(), null);
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
						text = "PW_CannotGoNoMaster".Translate();
					}
					else if (IntVec3Utility.DistanceTo(clickCell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
					{
						text = "PW_CannotGoTooFarFromMaster".Translate();
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
					text2 = "PW_CannotAttackNoMaster".Translate();
				}
				else if (IntVec3Utility.DistanceTo(clickCell, pawn.playerSettings.Master.Position) > PokemonMasterUtility.GetMasterObedienceRadius(pawn))
				{
					text2 = "PW_CannotAttackTooFarFromMaster".Translate();
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
			FloatMenuOption floatMenuOption3 = GotoLocationOption(clickCell, pawn);
			if (floatMenuOption3 != null)
			{
				opts.Add(floatMenuOption3);
			}
		}
	}
}
