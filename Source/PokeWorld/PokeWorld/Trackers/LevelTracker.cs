using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace PokeWorld
{
    public class LevelTracker : IExposable
    {
        public CompPokemon comp;
        public Pawn pokemonHolder;

        public List<Evolution> evolutions;
        public string expCategory;
        public int wildLevelMin;
        public int wildLevelMax;
        public int experience = 0;
        public int level = 1;
        public int totalExpForNextLevel;
        public bool flagEverstoneOn = false;
        public bool flagEverstoneAlertEvolution = true;
        public bool flagIsEvolving = false;
        public int evolutionCountDown = 300;
        public List<PawnKindDef> evolutionDefList = new List<PawnKindDef>();
        public bool canEvolve;

        public LevelTracker(CompPokemon comp)
        {
            this.comp = comp;
            pokemonHolder = comp.Pokemon;
            canEvolve = comp.canEvolve;
            evolutions = comp.evolutions;
            expCategory = comp.expCategory;
            wildLevelMin = comp.wildLevelMin;
            wildLevelMax = comp.wildLevelMax;

            level = (int)Rand.Range(wildLevelMin, wildLevelMax + 1);
            experience = 0;
            UpdateExpToNextLvl();
        }

        public IEnumerable<Gizmo> GetGizmos()
        {
            if (pokemonHolder.Faction == Faction.OfPlayer)
            {
                if (comp.canEvolve)
                {
                    if (!flagIsEvolving)
                    {
                        foreach (Evolution evo in evolutions)
                        {
                            if (evo.requirement == EvolutionRequirement.level && (evo.gender == Gender.None || pokemonHolder.gender == evo.gender) && PokeWorldSettings.GenerationAllowed(evo.pawnKind.race.GetCompProperties<CompProperties_Pokemon>().generation))
                            {
                                Command_Toggle command_Toggle = new Command_Toggle
                                {
                                    defaultLabel = (flagEverstoneOn ? "PW_TakeBackEverstone".Translate() : "PW_GiveEverstone".Translate()),
                                    defaultDesc = (flagEverstoneOn ? "PW_TakeBackEverstoneDesc".Translate() : "PW_GiveEverstoneDesc".Translate()),
                                    hotKey = KeyBindingDefOf.Misc5,
                                    icon = ContentFinder<Texture2D>.Get("UI/Gizmos/Everstone/Everstone"),                                   
                                    toggleAction = delegate
                                    {
                                        flagEverstoneOn = !flagEverstoneOn;
                                    },
                                    isActive = () => flagEverstoneOn
                                };
                                yield return command_Toggle;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Command_Action command_Action = new Command_Action();
                        command_Action.action = delegate
                        {
                            CancelEvolution(true);
                        };
                        command_Action.defaultLabel = "PW_StopEvolution".Translate();
                        command_Action.defaultDesc = "PW_StopEvolutionDesc".Translate();
                        command_Action.hotKey = KeyBindingDefOf.Misc5;
                        command_Action.icon = TexCommand.ClearPrioritizedWork;
                        yield return command_Action;
                    }
                }
            }
        }
        public void IncreaseExperience(int expAmount)
        {
            if (level >= 100)
            {
                level = 100;
                experience = 0;
            }
            else
            {
                experience += expAmount;
                TryGainLevel();
            }
        }
        public void TryGainLevel()
        {
            if (experience >= totalExpForNextLevel)
            {
                while (experience >= totalExpForNextLevel)
                {
                    level++;
                    if (pokemonHolder.Faction == Faction.OfPlayer)
                    {
                        comp.friendshipTracker.IncreaseFriendshipLevelUp();
                        MoteMaker.ThrowText(pokemonHolder.DrawPos, pokemonHolder.Map, "PW_LevelIncrease".Translate(level));
                        if (level % 10 == 0)
                        {
                            Messages.Message("PW_MessageLevelIncrease".Translate(pokemonHolder.Label, level), pokemonHolder, MessageTypeDefOf.NeutralEvent);
                        }
                    }                   
                    if (level >= 100)
                    {
                        level = 100;
                        experience = 0;
                        UpdateExpToNextLvl();
                        break;
                    }
                    experience -= totalExpForNextLevel;
                    UpdateExpToNextLvl();
                }
                comp.statTracker.UpdateStats();
                if (canEvolve && pokemonHolder.Faction == Faction.OfPlayer)
                {
                    TryEvolveAfterLevelUp();
                }
            }
        }
        public void UpdateExpToNextLvl()
        {
            totalExpForNextLevel = ExpTable.GetExpToNextLvl(expCategory, level);
        }
        public void TryEvolveAfterLevelUp()
        {
            if (!flagIsEvolving && pokemonHolder.Map.designationManager.DesignationOn(pokemonHolder, DesignationDefOf.Slaughter) == null)
            {
                foreach (Evolution evo in evolutions)
                {
                    if (PokeWorldSettings.GenerationAllowed(evo.pawnKind.race.GetCompProperties<CompProperties_Pokemon>().generation)
                    && evo.requirement == EvolutionRequirement.level && level >= evo.level
                    && comp.friendshipTracker.EvolutionAllowed(evo.friendship)
                    && (evo.gender == Gender.None || pokemonHolder.gender == evo.gender))                   
                    {
                        if (evo.otherRequirement != OtherEvolutionRequirement.none)
                        {
                            int attack = (int)pokemonHolder.GetStatValue(DefDatabase<StatDef>.GetNamed("PW_Attack"));
                            int defense = (int)pokemonHolder.GetStatValue(DefDatabase<StatDef>.GetNamed("PW_Defense"));
                            if(evo.otherRequirement == OtherEvolutionRequirement.attack && !(attack > defense))
                            {
                                continue;
                            }
                            else if (evo.otherRequirement == OtherEvolutionRequirement.defense && !(defense > attack))
                            {
                                continue;
                            }
                            else if(evo.otherRequirement == OtherEvolutionRequirement.balanced && !(attack == defense))
                            {
                                continue;
                            }
                        }
                        int currentMapTime = GenLocalDate.HourOfDay(pokemonHolder.Map);
                        if (evo.timeOfDay == TimeOfDay.Any
                        || (evo.timeOfDay == TimeOfDay.Day && currentMapTime >= 7 && currentMapTime < 19)
                        || (evo.timeOfDay == TimeOfDay.Night && (currentMapTime >= 19 || currentMapTime < 7)))
                        {
                            if (!flagEverstoneOn)
                            {
                                evolutionDefList.Add(evo.pawnKind);
                                BeginEvolutionProcess();
                            }
                            else if (flagEverstoneAlertEvolution)
                            {
                                Messages.Message("PW_MessageEverstonePreventsEvolution".Translate(pokemonHolder.Label), pokemonHolder, MessageTypeDefOf.NeutralEvent);
                                flagEverstoneAlertEvolution = false;
                            }
                        }
                    }
                }
            }
        }
        public void TryEvolveWithItem(Thing item)
        {
            if (!flagIsEvolving && pokemonHolder.Map.designationManager.DesignationOn(pokemonHolder, DesignationDefOf.Slaughter) == null)
            {
                foreach (Evolution evo in evolutions)
                {
                    if (evo.item == item.def)
                    {
                        evolutionDefList.Add(evo.pawnKind);
                        BeginEvolutionProcess();
                    }
                }
            }
        }

        public void BeginEvolutionProcess()
        {
            if (!flagIsEvolving && pokemonHolder.Map.designationManager.DesignationOn(pokemonHolder, DesignationDefOf.Slaughter) == null)
            {
                Messages.Message("PW_MessageEvolving".Translate(pokemonHolder.Label), pokemonHolder, MessageTypeDefOf.NeutralEvent);
                flagIsEvolving = true;
            }
        }

        public void UpdateEvolutionProcess()
        {
            if(pokemonHolder.Map.designationManager.DesignationOn(pokemonHolder, DesignationDefOf.Slaughter) != null)
            {
                CancelEvolution(true);
            }
            else
            {
                if (evolutionCountDown % 60 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        FleckMaker.ThrowDustPuff(pokemonHolder.Position, pokemonHolder.Map, 2f);
                    }
                }
                if (evolutionCountDown <= 0)
                {
                    flagIsEvolving = false;
                    Evolve(evolutionDefList);
                }
                evolutionCountDown -= 1;
            }      
        }

        public void CancelEvolution(bool scared = false)
        {
            Messages.Message(scared ? "PW_MessageScaredStoppedEvolving".Translate(pokemonHolder.Label) : "PW_MessageStoppedEvolving".Translate(pokemonHolder.Label), pokemonHolder, MessageTypeDefOf.NeutralEvent);
            flagIsEvolving = false;
            evolutionDefList.Clear();
            evolutionCountDown = 300;
        }

        public void Evolve(List<PawnKindDef> kindDefs)
        {
            Pawn preEvoPokemon = pokemonHolder;
            Faction faction = pokemonHolder.Faction;
            foreach (PawnKindDef evolutionKindDef in kindDefs)
            {
                Pawn postEvoPokemon = PawnGenerator.GeneratePawn(evolutionKindDef, faction);
                Copy(preEvoPokemon, postEvoPokemon);
                postEvoPokemon.health.Reset();
                GenSpawn.Spawn(postEvoPokemon, preEvoPokemon.Position, preEvoPokemon.Map);
                if(faction == Faction.OfPlayer)
                {
                    Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(postEvoPokemon.kindDef);
                }               
            }
            if (preEvoPokemon.inventory != null)
            {
                preEvoPokemon.inventory.DropAllNearPawn(preEvoPokemon.Position);
            }
            if (preEvoPokemon.carryTracker != null && preEvoPokemon.carryTracker.CarriedThing != null)
            {
                preEvoPokemon.carryTracker.TryDropCarriedThing(preEvoPokemon.Position, ThingPlaceMode.Near, out Thing carriedThing);
            }         
            if(preEvoPokemon.health.hediffSet != null)
            {
                foreach(Hediff hediff in preEvoPokemon.health.hediffSet.hediffs)
                {
                    if (hediff.def != null && hediff.def.countsAsAddedPartOrImplant)
                    {
                        BodyPartRecord part = hediff.Part;
                        MedicalRecipesUtility.SpawnThingsFromHediffs(preEvoPokemon, part, preEvoPokemon.Position, preEvoPokemon.Map);
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                FleckMaker.ThrowDustPuff(preEvoPokemon.Position, preEvoPokemon.Map, 2f);
            }
            preEvoPokemon.relations.ClearAllRelations();
            preEvoPokemon.Destroy();
        }

        private void Copy(Pawn pokemon, Pawn evolution)
        {
            CompPokemon postEvoComp = evolution.GetComp<CompPokemon>();
            postEvoComp.levelTracker.level = level;
            postEvoComp.levelTracker.experience = experience;
            postEvoComp.levelTracker.flagEverstoneOn = flagEverstoneOn;
            postEvoComp.levelTracker.UpdateExpToNextLvl();
            postEvoComp.friendshipTracker.friendship = comp.friendshipTracker.friendship;
            postEvoComp.friendshipTracker.flagMaxFriendshipMessage = comp.friendshipTracker.flagMaxFriendshipMessage;
            postEvoComp.ballDef = comp.ballDef;
            postEvoComp.statTracker.CopyPreEvoStat(comp);
            postEvoComp.statTracker.UpdateStats();
            postEvoComp.moveTracker.GetUnlockedMovesFromPreEvolution(comp);
            if (comp.shinyTracker.isShiny)
            {
                postEvoComp.shinyTracker.MakeShiny();
            }
            else
            {
                postEvoComp.shinyTracker.isShiny = false;
            }
            if (comp.formTracker != null && postEvoComp.formTracker != null)
            {
                postEvoComp.formTracker.TryInheritFormFromPreEvo(comp.formTracker);
            }
            if (evolution.RaceProps.hasGenders)
            {
                evolution.gender = pokemon.gender;
            }      
            evolution.records = pokemon.records;
            evolution.relations.ClearAllRelations();
            foreach (DirectPawnRelation relation in pokemon.relations.DirectRelations.ToList())
            {
                evolution.relations.AddDirectRelation(relation.def, relation.otherPawn);
            }
            foreach (Pawn relatedPawn in pokemon.relations.RelatedPawns.ToList())
            {
                foreach (DirectPawnRelation otherDirectRelation in relatedPawn.relations.DirectRelations.ToList())
                {
                    if (otherDirectRelation.otherPawn == pokemon && !otherDirectRelation.def.reflexive)
                    {
                        relatedPawn.relations.AddDirectRelation(otherDirectRelation.def, evolution);
                    }
                }
            }
            evolution.ageTracker.AgeBiologicalTicks = pokemon.ageTracker.AgeBiologicalTicks;
            evolution.ageTracker.AgeChronologicalTicks = pokemon.ageTracker.AgeChronologicalTicks;
            evolution.ageTracker.BirthAbsTicks = pokemon.ageTracker.BirthAbsTicks;
            foreach (TrainableDef td in DefDatabase<TrainableDef>.AllDefs)
            {
                if (evolution.training.CanBeTrained(td))
                {
                    if (pokemon.training.HasLearned(td))
                    {
                        evolution.training.Train(td, null, true);
                    }
                    if (pokemon.training.GetWanted(td))
                    {
                        evolution.training.SetWantedRecursive(td, true);
                    }
                }
            }
            evolution.playerSettings.animalsReleased = pokemon.playerSettings.animalsReleased;
            evolution.playerSettings.AreaRestriction = pokemon.playerSettings.AreaRestriction;
            evolution.playerSettings.displayOrder = pokemon.playerSettings.displayOrder;
            evolution.playerSettings.followDrafted = pokemon.playerSettings.followDrafted;
            evolution.playerSettings.followFieldwork = pokemon.playerSettings.followFieldwork;
            evolution.playerSettings.hostilityResponse = pokemon.playerSettings.hostilityResponse;
            evolution.playerSettings.joinTick = pokemon.playerSettings.joinTick;
            evolution.playerSettings.Master = pokemon.playerSettings.Master;
            evolution.playerSettings.medCare = pokemon.playerSettings.medCare;
            if (pokemon.Name != null && !pokemon.Name.Numerical)
            {
                evolution.Name = pokemon.Name;
            }
        }

        public void LevelTick()
        {
            if (flagIsEvolving)
            {
                UpdateEvolutionProcess();
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref experience, "PW_experience", 0);
            Scribe_Values.Look(ref totalExpForNextLevel, "PW_totalExpForNextLevel", 1);
            Scribe_Values.Look(ref level, "PW_level", 1);
            Scribe_Values.Look(ref flagEverstoneOn, "PW_flagEverstoneOn", false);
            Scribe_Values.Look(ref flagEverstoneAlertEvolution, "PW_flagEverstoneAlertEvolution", false);
            Scribe_Values.Look(ref flagIsEvolving, "PW_flagIsEvolving", false);
            Scribe_Values.Look(ref evolutionCountDown, "PW_evolutionCountDown", 300);
            Scribe_Collections.Look(ref evolutionDefList, "PW_evolutionDefList", LookMode.Def);
        }
    }
}
