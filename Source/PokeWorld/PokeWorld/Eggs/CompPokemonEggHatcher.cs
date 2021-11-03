using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using RimWorld.Planet;
using UnityEngine;

namespace PokeWorld
{
    public class CompPokemonEggHatcher : CompHatcher
    {
        private float pokemonEggGestateProgress;
        public override void CompTick()
        {
            if (!TemperatureDamaged)
            {
                float num = 1f / (Props.hatcherDaystoHatch * 60000f);
                pokemonEggGestateProgress += num;
                if (pokemonEggGestateProgress > 1f)
                {
                    HatchPokemon();
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref pokemonEggGestateProgress, "PW_pokemonEggGestateProgress", 0f);
        }
        public override string CompInspectStringExtra()
        {

            if (!TemperatureDamaged)
            {
                if (Props.hatcherDaystoHatch - (Props.hatcherDaystoHatch * pokemonEggGestateProgress) < 1)
                {
                    return "PW_CompPokemonEggHatcherSoon".Translate();
                }
                else if (Props.hatcherDaystoHatch - (Props.hatcherDaystoHatch * pokemonEggGestateProgress) < 5)
                {
                    return "PW_CompPokemonEggHatcherClose".Translate();
                }
                else if (Props.hatcherDaystoHatch - (Props.hatcherDaystoHatch * pokemonEggGestateProgress) < 15)
                {
                    return "PW_CompPokemonEggHatcherNotClose".Translate();
                }
                else
                {
                    return "PW_CompPokemonEggHatcherLong".Translate();
                }
            }
            return null;
        }
        public override string TransformLabel(string label)
        {
            return "PW_PokemonEgg".Translate();
        }
        public void HatchPokemon()
        {
            try
            {
                PawnGenerationRequest request = new PawnGenerationRequest(Props.hatcherPawn, hatcheeFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: true);
                for (int i = 0; i < parent.stackCount; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    CompPokemon comp = pawn.TryGetComp<CompPokemon>();
                    if(comp != null)
                    {
                        comp.levelTracker.level = 1;
                        comp.levelTracker.UpdateExpToNextLvl();
                        comp.statTracker.UpdateStats();
                    }
                    if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent))
                    {
                        if (pawn != null)
                        {
                            if (hatcheeParent != null)
                            {
                                if (pawn.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
                                {
                                    pawn.playerSettings.AreaRestriction = hatcheeParent.playerSettings.AreaRestriction;
                                }
                                if (pawn.RaceProps.IsFlesh)
                                {
                                    pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
                                }
                                if(comp.formTracker != null)
                                {
                                    if(hatcheeParent.TryGetComp<CompDittoEggLayer>() != null && otherParent != null)
                                    {
                                        CompPokemon comp2 = otherParent.TryGetComp<CompPokemon>();
                                        if (comp2 != null && comp2.formTracker != null)
                                        {
                                            comp.formTracker.TryInheritFormFromParent(comp2.formTracker);
                                        }
                                    }
                                    else
                                    {
                                        CompPokemon comp3 = hatcheeParent.TryGetComp<CompPokemon>();
                                        if (comp3 != null && comp3.formTracker != null)
                                        {
                                            comp.formTracker.TryInheritFormFromParent(comp3.formTracker);
                                        }
                                    }                                
                                }
                            }
                            if (otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && pawn.RaceProps.IsFlesh)
                            {
                                pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
                            }
                            if (pawn.Faction == Faction.OfPlayer)
                            {
                                Find.World.GetComponent<PokedexManager>().AddPokemonKindCaught(pawn.kindDef);
                            }
                        }
                        if (parent.Spawned)
                        {
                            FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_AmnioticFluid);
                        }                      
                    }
                    else
                    {
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                    }
                }
            }
            finally
            {
                parent.Destroy();
            }
        }

    }
    public class CompProperties_PokemonEggHatcher : CompProperties_Hatcher
    {
        public CompProperties_PokemonEggHatcher()
        {
            this.compClass = typeof(CompPokemonEggHatcher);
        }
    }
}
