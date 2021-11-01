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
	[HarmonyPatch(typeof(Toils_Recipe))]
	[HarmonyPatch("FinishRecipeAndStartStoringProduct")]
	class FinishRecipeAndStartStoringProduct_Patch
	{
		public static void Prefix(ref Toil __state)
		{			
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				RecipeDef recipe = toil.actor.jobs.curJob.RecipeDef;
				if (recipe != null && recipe.products.Count > 0 && recipe.products[0].thingDef.HasComp(typeof(CompPokemon)) && (recipe.products[0].thingDef.comps.Find((CompProperties x) => x.compClass == typeof(CompPokemon)) as CompProperties_Pokemon).attributes.Contains(PokemonAttribute.Makeable))
				{
					Pawn actor = toil.actor;
					Job curJob = actor.jobs.curJob;
					JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor.jobs.curDriver;
					if (curJob.RecipeDef.workSkill != null && !curJob.RecipeDef.UsesUnfinishedThing)
					{
						float xp = (float)jobDriver_DoBill.ticksSpentDoingRecipeWork * 0.1f * curJob.RecipeDef.workSkillLearnFactor;
						actor.skills.GetSkill(curJob.RecipeDef.workSkill).Learn(xp);
					}
					List<Thing> ingredients = CalculateIngredients(curJob, actor);
					Thing dominantIngredient = CalculateDominantIngredient(curJob, ingredients);
					List<Thing> list = GenRecipe.MakeRecipeProducts(curJob.RecipeDef, actor, ingredients, dominantIngredient, jobDriver_DoBill.BillGiver).ToList();
					ConsumeIngredients(ingredients, curJob.RecipeDef, actor.Map);
					curJob.bill.Notify_IterationCompleted(actor, ingredients);
					RecordsUtility.Notify_BillDone(actor, list);
					UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					if (curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff) >= 10000f && list.Count > 0)
					{
						TaleRecorder.RecordTale(TaleDefOf.CompletedLongCraftingProject, actor, list[0].GetInnerIfMinified().def);
					}
					if (list.Any())
					{
						Find.QuestManager.Notify_ThingsProduced(actor, list);
					}
					if (list.Count == 0)
					{
						actor.jobs.EndCurrentJob(JobCondition.Succeeded);
					}
					bool isFossil;
					string verb;
					if (curJob.RecipeDef.products[0].thingDef.defName == "PW_Porygon")
					{
						verb = "made";
						isFossil = false;
					}
					else if (curJob.RecipeDef.products[0].thingDef.defName == "PW_Spiritomb")
					{
						verb = "unleashed";
						isFossil = false;
					}
					else
					{
						verb = "resurrected";
						isFossil = true;
					}
					Pawn revivedPokemon = PokemonGeneratorUtility.GenerateAndSpawnNewPokemon(DefDatabase<PawnKindDef>.GetNamed(curJob.RecipeDef.products[0].thingDef.defName), Faction.OfPlayer, actor.Position, actor.Map, actor, true, isFossil);
					Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(DefDatabase<PawnKindDef>.GetNamed(curJob.RecipeDef.products[0].thingDef.defName));
					Messages.Message(actor.ToString() + " succesfully " + verb + " a " + revivedPokemon.KindLabel + "!", revivedPokemon, MessageTypeDefOf.PositiveEvent);
				}
                else
                {
					Pawn actor = toil.actor;
					Job curJob = actor.jobs.curJob;
					JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor.jobs.curDriver;
					if (curJob.RecipeDef.workSkill != null && !curJob.RecipeDef.UsesUnfinishedThing)
					{
						float xp = (float)jobDriver_DoBill.ticksSpentDoingRecipeWork * 0.1f * curJob.RecipeDef.workSkillLearnFactor;
						actor.skills.GetSkill(curJob.RecipeDef.workSkill).Learn(xp);
					}
					List<Thing> ingredients = CalculateIngredients(curJob, actor);
					Thing dominantIngredient = CalculateDominantIngredient(curJob, ingredients);
					List<Thing> list = GenRecipe.MakeRecipeProducts(curJob.RecipeDef, actor, ingredients, dominantIngredient, jobDriver_DoBill.BillGiver).ToList();
					ConsumeIngredients(ingredients, curJob.RecipeDef, actor.Map);
					curJob.bill.Notify_IterationCompleted(actor, ingredients);
					RecordsUtility.Notify_BillDone(actor, list);
					UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					if (curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff) >= 10000f && list.Count > 0)
					{
						TaleRecorder.RecordTale(TaleDefOf.CompletedLongCraftingProject, actor, list[0].GetInnerIfMinified().def);
					}
					if (list.Any())
					{
						Find.QuestManager.Notify_ThingsProduced(actor, list);
					}
					if (list.Count == 0)
					{
						actor.jobs.EndCurrentJob(JobCondition.Succeeded);
					}
					else if (curJob.bill.GetStoreMode() == BillStoreModeDefOf.DropOnFloor)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (!GenPlace.TryPlaceThing(list[i], actor.Position, actor.Map, ThingPlaceMode.Near))
							{
								Log.Error(string.Concat(actor, " could not drop recipe product ", list[i], " near ", actor.Position));
							}
						}
						actor.jobs.EndCurrentJob(JobCondition.Succeeded);
					}
					else
					{
						if (list.Count > 1)
						{
							for (int j = 1; j < list.Count; j++)
							{
								if (!GenPlace.TryPlaceThing(list[j], actor.Position, actor.Map, ThingPlaceMode.Near))
								{
									Log.Error(string.Concat(actor, " could not drop recipe product ", list[j], " near ", actor.Position));
								}
							}
						}
						IntVec3 foundCell = IntVec3.Invalid;
						if (curJob.bill.GetStoreMode() == BillStoreModeDefOf.BestStockpile)
						{
							StoreUtility.TryFindBestBetterStoreCellFor(list[0], actor, actor.Map, StoragePriority.Unstored, actor.Faction, out foundCell);
						}
						else if (curJob.bill.GetStoreMode() == BillStoreModeDefOf.SpecificStockpile)
						{
							StoreUtility.TryFindBestBetterStoreCellForIn(list[0], actor, actor.Map, StoragePriority.Unstored, actor.Faction, curJob.bill.GetStoreZone().slotGroup, out foundCell);
						}
						else
						{
							Log.ErrorOnce("Unknown store mode", 9158246);
						}
						if (foundCell.IsValid)
						{
							actor.carryTracker.TryStartCarry(list[0]);
							curJob.targetB = foundCell;
							curJob.targetA = list[0];
							curJob.count = 99999;
						}
						else
						{
							if (!GenPlace.TryPlaceThing(list[0], actor.Position, actor.Map, ThingPlaceMode.Near))
							{
								Log.Error(string.Concat("Bill doer could not drop product ", list[0], " near ", actor.Position));
							}
							actor.jobs.EndCurrentJob(JobCondition.Succeeded);
						}
					}
				}				
			};
			__state = toil;
		}

		public static void Postfix(ref Toil __state, ref Toil __result)
		{
			__result = __state;
		}



		private static List<Thing> CalculateIngredients(Job job, Pawn actor)
		{
			UnfinishedThing unfinishedThing = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
			if (unfinishedThing != null)
			{
				List<Thing> ingredients = unfinishedThing.ingredients;
				job.RecipeDef.Worker.ConsumeIngredient(unfinishedThing, job.RecipeDef, actor.Map);
				job.placedThings = null;
				return ingredients;
			}
			List<Thing> list = new List<Thing>();
			if (job.placedThings != null)
			{
				for (int i = 0; i < job.placedThings.Count; i++)
				{
					if (job.placedThings[i].Count <= 0)
					{
						Log.Error(string.Concat("PlacedThing ", job.placedThings[i], " with count ", job.placedThings[i].Count, " for job ", job));
						continue;
					}
					Thing thing = ((job.placedThings[i].Count >= job.placedThings[i].thing.stackCount) ? job.placedThings[i].thing : job.placedThings[i].thing.SplitOff(job.placedThings[i].Count));
					job.placedThings[i].Count = 0;
					if (list.Contains(thing))
					{
						Log.Error("Tried to add ingredient from job placed targets twice: " + thing);
						continue;
					}
					list.Add(thing);
					if (job.RecipeDef.autoStripCorpses)
					{
						(thing as IStrippable)?.Strip();
					}
				}
			}
			job.placedThings = null;
			return list;
		}

		private static Thing CalculateDominantIngredient(Job job, List<Thing> ingredients)
		{
			UnfinishedThing uft = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
			if (uft != null && uft.def.MadeFromStuff)
			{
				return uft.ingredients.First((Thing ing) => ing.def == uft.Stuff);
			}
			if (!ingredients.NullOrEmpty())
			{
				if (job.RecipeDef.productHasIngredientStuff)
				{
					return ingredients[0];
				}
				if (job.RecipeDef.products.Any((ThingDefCountClass x) => x.thingDef.MadeFromStuff))
				{
					return ingredients.Where((Thing x) => x.def.IsStuff).RandomElementByWeight((Thing x) => x.stackCount);
				}
				return ingredients.RandomElementByWeight((Thing x) => x.stackCount);
			}
			return null;
		}

		private static void ConsumeIngredients(List<Thing> ingredients, RecipeDef recipe, Map map)
		{
			for (int i = 0; i < ingredients.Count; i++)
			{
				recipe.Worker.ConsumeIngredient(ingredients[i], recipe, map);
			}
		}

	}
}
