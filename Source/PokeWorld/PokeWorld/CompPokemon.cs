using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace PokeWorld
{
    public class CompPokemon : ThingComp
    {

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            PokemonComponentsUtility.CreateInitialComponents(this);         
            statTracker.UpdateStats();                  
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!Pokemon.Position.Fogged(Pokemon.Map))
            {
                PokedexManager pokedex = Find.World.GetComponent<PokedexManager>();
                if (!pokedex.IsPokemonSeen(Pokemon.kindDef))
                {
                    pokedex.AddPokemonKindSeen(Pokemon.kindDef);
                }
            }           
            statTracker.UpdateStats();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (levelTracker != null)
            {
                levelTracker.LevelTick();
            }
            if (friendshipTracker != null)
            {
                friendshipTracker.FriendshipTick();
            }
            if (formTracker != null)
            {
                formTracker.FormTick();
            }
            if (tryCatchCooldown > 0)
            {
                tryCatchCooldown -= 1;
            }
            if (inBall == true && Pokemon.Spawned)
            {
                inBall = false;
            }
            if (flagIsPokedexSeen == null)
            {
                flagIsPokedexSeen = Find.World.GetComponent<PokedexManager>().IsPokemonSeen(Pokemon.kindDef);
            }
            else if (!(bool)flagIsPokedexSeen && Pokemon.Spawned && !Pokemon.Position.Fogged(Pokemon.Map))
            {
                Find.World.GetComponent<PokedexManager>().AddPokemonKindSeen(Pokemon.kindDef);
                flagIsPokedexSeen = true;
            }
            if (flagIsPokedexCaught == null)
            {
                flagIsPokedexCaught = Find.World.GetComponent<PokedexManager>().IsPokemonCaught(Pokemon.kindDef);
            }
            else if(!(bool)flagIsPokedexCaught && Pokemon.Faction == Faction.OfPlayer)
            {
                Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(Pokemon.kindDef);
                flagIsPokedexCaught = true;
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (shinyTracker != null)
            {
                shinyTracker.ShinyTickRare();
            }
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (parent.Faction != null && parent.Faction == Faction.OfPlayer)
            {
                NatureDef nature = statTracker.nature;
                string natureLabel = nature.label;
                string natureDescription = "PW_StatNatureDesc".Translate() + "\n\n";
                if (nature.increasedStat != nature.decreasedStat)
                {
                    natureDescription += "PW_StatNatureChangeDesc".Translate(nature.increasedStat.label, nature.decreasedStat.label);
                }
                else
                {
                    natureDescription += "PW_StatNatureNoChangeDesc".Translate();
                }
                yield return new StatDrawEntry(DefDatabase<StatCategoryDef>.GetNamed("PW_PokeWorldStat"), "PW_Nature".Translate(), natureLabel, natureDescription, 4050);
            }
                       
            string eggGroupLabel = "";
            string eggGroupDescription = "PW_StatEggGroupDesc".Translate() + "\n";
            string typeLabel = "";
            string typeDescription = "PW_StatTypeDesc".Translate() + "\n";
            if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(parent.def.race.AnyPawnKind))
            {
                foreach (EggGroupDef eggGroupDef in eggGroups)
                {
                    if (eggGroupLabel.Length > 0)
                    {
                        eggGroupLabel += ", ";
                    }
                    eggGroupLabel += eggGroupDef.LabelCap;
                    eggGroupDescription += "\n" + "PW_EggGroupLabelAndDesc".Translate(eggGroupDef.LabelCap, eggGroupDef.description);
                }
                for(int x = 0; x < types.Count(); x++)
                {
                    if (typeLabel.Length > 0)
                    {
                        typeLabel += ", ";
                    }
                    string text;                   
                    if (x == 0)
                    {
                        text = "PW_TypePrimary".Translate();
                    }
                    else if (x == 1)
                    {
                        text = "PW_TypeSecondary".Translate();
                    }
                    else
                    {
                        text = "PW_Type".Translate();
                    }                   
                    typeLabel += types[x].LabelCap;
                    typeDescription += "\n" + "PW_TypeName".Translate(text, types[x].LabelCap);
                }
            }
            else
            {
                eggGroupLabel = "PW_StatUnknownEggGroup".Translate();
                typeLabel = "PW_StatUnknownType".Translate();
            }                  
            yield return new StatDrawEntry(DefDatabase<StatCategoryDef>.GetNamed("PW_PokeWorldStat"), "PW_EggGroup".Translate(), eggGroupLabel, eggGroupDescription, 4049);           
            yield return new StatDrawEntry(DefDatabase<StatCategoryDef>.GetNamed("PW_PokeWorldStat"), "PW_Type".Translate(), typeLabel, typeDescription, 4054);           
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (Pokemon.Faction == Faction.OfPlayer)
            {
                if (Pokemon.Map.designationManager.DesignationOn(Pokemon, DefDatabase<DesignationDef>.GetNamed("PW_PutInBall")) == null)
                {
                    Command_Action command_PutInBall = new Command_Action();
                    command_PutInBall.defaultLabel = "PW_DesignationPutInPokeball".Translate();
                    command_PutInBall.defaultDesc = "PW_DesignationPutInPokeballDesc".Translate();
                    command_PutInBall.hotKey = KeyBindingDefOf.Misc2;
                    command_PutInBall.icon = ContentFinder<Texture2D>.Get("Things/Item/Utility/Balls/PokeBall");
                    command_PutInBall.action = delegate ()
                    {
                        PutInBallUtility.UpdatePutInBallDesignation(Pokemon);
                    };
                    yield return command_PutInBall;
                }
            }
            if (levelTracker != null)
            {
                foreach (Gizmo item in levelTracker.GetGizmos())
                {
                    yield return item;
                }
            }
            if (formTracker != null)
            {
                foreach (Gizmo item in formTracker.GetGizmos())
                {
                    yield return item;
                }
            }
            if (moveTracker != null)
            {
                foreach (Gizmo item in moveTracker.GetGizmos())
                {
                    yield return item;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            PokemonComponentsUtility.ExposeData(this);                     
            Scribe_Values.Look(ref wantPutInBall, "wantPutInBall", defaultValue: false);
            Scribe_Values.Look(ref inBall, "inBall", defaultValue: false);
            Scribe_Defs.Look(ref ballDef, "ballDef");
            Scribe_Values.Look(ref tryCatchCooldown, "tryCatchCooldown", defaultValue: 0);
            Scribe_Values.Look(ref flagIsPokedexSeen, "flagIsPokedexSeen", defaultValue: false);
            Scribe_Values.Look(ref flagIsPokedexCaught, "flagIsPokedexCaught", defaultValue: false);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (PokemonMasterUtility.IsPokemonMasterDrafted(Pokemon))
            {
                PokemonMasterUtility.DrawObedienceRadiusRingAroundMaster(Pokemon);
                if (Pokemon.pather.curPath != null)
                {
                    Pokemon.pather.curPath.DrawPath(Pokemon);
                }
                Pokemon.jobs.DrawLinesBetweenTargets();
            }
        }
        public override string CompInspectStringExtra()
        {
            string text;
            if (Pokemon.Faction != null && Pokemon.Faction.IsPlayer)
            {
                text = "PW_InspectLevelExperiencePlayerPokemon".Translate(levelTracker.level, levelTracker.experience, levelTracker.totalExpForNextLevel);
                text += '\n' + friendshipTracker.GetStatement();
            }
            else
            {
                text = "PW_InspectLevelExperienceNonPlayerPokemon".Translate(levelTracker.level);
            }
            return text;
        }
        public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            base.PrePreTraded(action, playerNegotiator, trader);
            if (levelTracker.flagIsEvolving)
            {
                levelTracker.CancelEvolution();
            }
            if (action == TradeAction.PlayerBuys)
            {
                Pokemon.SetFaction(Faction.OfPlayer, playerNegotiator);
                Pokemon.training.Train(DefDatabase<TrainableDef>.GetNamed("Obedience"), playerNegotiator, true);
                Pokemon.training.SetWantedRecursive(DefDatabase<TrainableDef>.GetNamed("Obedience"), true);
                Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(Pokemon.kindDef);
            }
            else if(action == TradeAction.PlayerSells)
            {
                Pokemon.SetFaction(trader.Faction);
            }
        }
              
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            shinyTracker = null;
            levelTracker = null;
            friendshipTracker = null;
            statTracker = null;
        }

        public CompProperties_Pokemon Props => (CompProperties_Pokemon)this.props;
        public int pokedexNumber => Props.pokedexNumber;
        public int generation => Props.generation;
        public List<TypeDef> types => Props.types;
        public bool starter => Props.starter;
        public int rarity => Props.rarity;
        public bool canEvolve => Props.canEvolve;
        public int evolutionLine => Props.evolutionLine;
        public List<Evolution> evolutions => Props.evolutions;
        public List<Move> moves => Props.moves;
        public List<PokemonAttribute> attributes => Props.attributes;
        public string expCategory => Props.expCategory;
        public int wildLevelMin => Props.wildLevelMin;
        public int wildLevelMax => Props.wildLevelMax;
        public List<EggGroupDef> eggGroups => Props.eggGroups;
        public int baseHP => Props.baseHP;
        public int baseAttack => Props.baseAttack;
        public int baseDefense => Props.baseDefense;
        public int baseSpAttack => Props.baseSpAttack;
        public int baseSpDefense => Props.baseSpDefense;
        public int baseSpeed => Props.baseSpeed;
        public int baseFriendship => Props.baseFriendship;
        public List<EVYield> EVYields => Props.EVYields;
        public int catchRate => Props.catchRate;
        public float femaleRatio => Props.femaleRatio;     
        public float shinyChance => Props.shinyChance;
        public FormChangerCondition formChangerCondition => Props.formChangerCondition;
        public bool showFormLabel => Props.showFormLabel;
        public List<PokemonForm> forms => Props.forms;

        public ShinyTracker shinyTracker;
        public LevelTracker levelTracker;
        public FriendshipTracker friendshipTracker;
        public StatTracker statTracker;
        public MoveTracker moveTracker;
        public FormTracker formTracker;

        public Pawn Pokemon => (Pawn)parent;
        public bool wantPutInBall = false;
        public bool inBall;
        public ThingDef ballDef = DefDatabase<ThingDef>.GetNamed("PW_CryptosleepPokeBall");
        public bool? flagIsPokedexSeen;
        public bool? flagIsPokedexCaught;

        public int tryCatchCooldown = 0;
    }

    public class CompProperties_Pokemon : CompProperties
    {
        public int pokedexNumber;
        public int generation;
        public List<TypeDef> types;
        public bool starter;
        public int rarity;
        public bool canEvolve;
        public int evolutionLine;
        public List<Evolution> evolutions;
        public List<Move> moves;
        public List<PokemonAttribute> attributes = new List<PokemonAttribute>();
        public string expCategory;
        public int wildLevelMin;
        public int wildLevelMax;
        public List<EggGroupDef> eggGroups;
        public int baseFriendship;
        public float femaleRatio;
        public float shinyChance;
        public FormChangerCondition formChangerCondition;
        public bool showFormLabel = false;
        public List<PokemonForm> forms;
        public int baseHP;
        public int baseAttack;
        public int baseDefense;
        public int baseSpAttack;
        public int baseSpDefense;
        public int baseSpeed;
        public List<EVYield> EVYields;
        public int catchRate;

        public CompProperties_Pokemon()
        {
            this.compClass = typeof(CompPokemon);
        }

        public CompProperties_Pokemon(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
